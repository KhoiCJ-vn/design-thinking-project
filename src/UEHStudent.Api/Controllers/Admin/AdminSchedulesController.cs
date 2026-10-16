using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Services;

namespace UEHStudent.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/schedules")]
[Authorize(Roles = "Admin")]
public class AdminSchedulesController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<AdminSchedulesController> _logger;

    public AdminSchedulesController(UEHStudentDbContext context, ILogger<AdminSchedulesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRules()
    {
        var rules = await _context.ClassScheduleRules
            .AsNoTracking()
            .Include(r => r.ClassSection)
                .ThenInclude(cs => cs.Course)
            .OrderBy(r => r.ClassSectionId)
            .ThenBy(r => r.DayOfWeek)
            .Select(r => new
            {
                r.ScheduleRuleId,
                r.ClassSectionId,
                ClassSectionCode = r.ClassSection.ClassSectionCode,
                CourseName = r.ClassSection.Course.CourseName,
                r.DayOfWeek,
                DayOfWeekName = DateTimeHelper.GetVietnameseDayOfWeekName(r.DayOfWeek),
                StartTime = r.StartTime.ToString(@"hh\:mm"),
                EndTime = r.EndTime.ToString(@"hh\:mm"),
                r.Room,
                r.Instructor,
                EffectiveStartDate = r.EffectiveStartDate.ToString("yyyy-MM-dd"),
                EffectiveEndDate = r.EffectiveEndDate.ToString("yyyy-MM-dd"),
                r.IsActive
            })
            .ToListAsync();

        return Ok(rules);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRule([FromBody] CreateScheduleRuleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!TimeOnly.TryParse(request.StartTime, out var startTime) ||
            !TimeOnly.TryParse(request.EndTime, out var endTime))
        {
            return BadRequest(new { message = "Giờ bắt đầu hoặc giờ kết thúc không đúng định dạng (HH:mm)." });
        }

        if (startTime >= endTime)
        {
            return BadRequest(new { message = "Giờ bắt đầu phải nhỏ hơn giờ kết thúc." });
        }

        if (!DateOnly.TryParse(request.EffectiveStartDate, out var startDate) ||
            !DateOnly.TryParse(request.EffectiveEndDate, out var endDate))
        {
            return BadRequest(new { message = "Ngày hiệu lực không đúng định dạng (yyyy-MM-dd)." });
        }

        if (startDate > endDate)
        {
            return BadRequest(new { message = "Ngày bắt đầu hiệu lực không được lớn hơn ngày kết thúc." });
        }

        var classSection = await _context.ClassSections.FindAsync(request.ClassSectionId);
        if (classSection == null)
        {
            return NotFound(new { message = $"Không tìm thấy lớp học phần ID = {request.ClassSectionId}" });
        }

        var rule = new ClassScheduleRule
        {
            ClassSectionId = request.ClassSectionId,
            DayOfWeek = request.DayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            Room = request.Room?.Trim(),
            Instructor = request.Instructor?.Trim(),
            EffectiveStartDate = startDate,
            EffectiveEndDate = endDate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ClassScheduleRules.Add(rule);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tạo quy tắc lịch học thành công.", scheduleRuleId = rule.ScheduleRuleId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRule(int id, [FromBody] UpdateScheduleRuleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var rule = await _context.ClassScheduleRules.FindAsync(id);
        if (rule == null)
            return NotFound(new { message = "Không tìm thấy quy tắc lịch học." });

        if (!TimeOnly.TryParse(request.StartTime, out var startTime) ||
            !TimeOnly.TryParse(request.EndTime, out var endTime))
        {
            return BadRequest(new { message = "Giờ học không hợp lệ." });
        }

        if (startTime >= endTime)
        {
            return BadRequest(new { message = "Giờ bắt đầu phải nhỏ hơn giờ kết thúc." });
        }

        if (!DateOnly.TryParse(request.EffectiveStartDate, out var startDate) ||
            !DateOnly.TryParse(request.EffectiveEndDate, out var endDate))
        {
            return BadRequest(new { message = "Ngày hiệu lực không hợp lệ." });
        }

        rule.ClassSectionId = request.ClassSectionId;
        rule.DayOfWeek = request.DayOfWeek;
        rule.StartTime = startTime;
        rule.EndTime = endTime;
        rule.Room = request.Room?.Trim();
        rule.Instructor = request.Instructor?.Trim();
        rule.EffectiveStartDate = startDate;
        rule.EffectiveEndDate = endDate;
        rule.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cập nhật quy tắc lịch học thành công." });
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var rule = await _context.ClassScheduleRules.FindAsync(id);
        if (rule == null)
            return NotFound(new { message = "Không tìm thấy quy tắc lịch học." });

        rule.IsActive = !rule.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = rule.IsActive ? "Đã kích hoạt lại quy tắc lịch học." : "Đã ngừng áp dụng quy tắc lịch học.", isActive = rule.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRule(int id)
    {
        var rule = await _context.ClassScheduleRules.FindAsync(id);
        if (rule == null)
            return NotFound(new { message = "Không tìm thấy quy tắc lịch học." });

        // Xóa an toàn: Ngừng hiển thị hoặc xóa vật lý
        _context.ClassScheduleRules.Remove(rule);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xóa quy tắc lịch học thành công." });
    }
}
