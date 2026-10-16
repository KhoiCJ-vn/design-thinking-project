using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;

namespace UEHStudent.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/exams")]
[Authorize(Roles = "Admin")]
public class AdminExamsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<AdminExamsController> _logger;

    public AdminExamsController(UEHStudentDbContext context, ILogger<AdminExamsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllExams()
    {
        var exams = await _context.ExamSchedules
            .AsNoTracking()
            .Include(e => e.ClassSection)
                .ThenInclude(cs => cs.Course)
            .OrderBy(e => e.ExamDate)
            .ThenBy(e => e.StartTime)
            .Select(e => new
            {
                e.ExamScheduleId,
                e.ClassSectionId,
                ClassSectionCode = e.ClassSection.ClassSectionCode,
                CourseName = e.ClassSection.Course.CourseName,
                ExamDate = e.ExamDate.ToString("yyyy-MM-dd"),
                StartTime = e.StartTime.ToString(@"hh\:mm"),
                EndTime = e.EndTime.HasValue ? e.EndTime.Value.ToString(@"hh\:mm") : null,
                e.Room,
                e.Location,
                e.ExamType,
                e.ExamForm,
                e.Notes,
                e.IsActive
            })
            .ToListAsync();

        return Ok(exams);
    }

    [HttpPost]
    public async Task<IActionResult> CreateExam([FromBody] CreateExamScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!DateOnly.TryParse(request.ExamDate, out var examDate))
        {
            return BadRequest(new { message = "Ngày thi không đúng định dạng (yyyy-MM-dd)." });
        }

        if (!TimeOnly.TryParse(request.StartTime, out var startTime))
        {
            return BadRequest(new { message = "Giờ thi không đúng định dạng (HH:mm)." });
        }

        TimeOnly? endTime = null;
        if (!string.IsNullOrEmpty(request.EndTime) && TimeOnly.TryParse(request.EndTime, out var et))
        {
            endTime = et;
        }

        var classSection = await _context.ClassSections.FindAsync(request.ClassSectionId);
        if (classSection == null)
        {
            return NotFound(new { message = $"Không tìm thấy lớp học phần ID = {request.ClassSectionId}" });
        }

        var exam = new ExamSchedule
        {
            ClassSectionId = request.ClassSectionId,
            ExamDate = examDate,
            StartTime = startTime,
            EndTime = endTime,
            Room = request.Room?.Trim(),
            Location = request.Location?.Trim() ?? "UEH Nguyễn Văn Linh",
            ExamType = request.ExamType.Trim(),
            ExamForm = request.ExamForm?.Trim(),
            Notes = request.Notes?.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.ExamSchedules.Add(exam);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tạo lịch thi thành công.", examScheduleId = exam.ExamScheduleId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateExam(int id, [FromBody] UpdateExamScheduleRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var exam = await _context.ExamSchedules.FindAsync(id);
        if (exam == null)
            return NotFound(new { message = "Không tìm thấy lịch thi." });

        if (!DateOnly.TryParse(request.ExamDate, out var examDate))
        {
            return BadRequest(new { message = "Ngày thi không đúng định dạng." });
        }

        if (!TimeOnly.TryParse(request.StartTime, out var startTime))
        {
            return BadRequest(new { message = "Giờ thi không đúng định dạng." });
        }

        TimeOnly? endTime = null;
        if (!string.IsNullOrEmpty(request.EndTime) && TimeOnly.TryParse(request.EndTime, out var et))
        {
            endTime = et;
        }

        exam.ClassSectionId = request.ClassSectionId;
        exam.ExamDate = examDate;
        exam.StartTime = startTime;
        exam.EndTime = endTime;
        exam.Room = request.Room?.Trim();
        exam.Location = request.Location?.Trim();
        exam.ExamType = request.ExamType.Trim();
        exam.ExamForm = request.ExamForm?.Trim();
        exam.Notes = request.Notes?.Trim();
        exam.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cập nhật lịch thi thành công." });
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var exam = await _context.ExamSchedules.FindAsync(id);
        if (exam == null)
            return NotFound(new { message = "Không tìm thấy lịch thi." });

        exam.IsActive = !exam.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = exam.IsActive ? "Đã kích hoạt lại lịch thi." : "Đã ngừng hiển thị lịch thi.", isActive = exam.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteExam(int id)
    {
        var exam = await _context.ExamSchedules.FindAsync(id);
        if (exam == null)
            return NotFound(new { message = "Không tìm thấy lịch thi." });

        _context.ExamSchedules.Remove(exam);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xóa lịch thi thành công." });
    }
}
