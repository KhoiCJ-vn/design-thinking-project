using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;

namespace UEHStudent.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/events")]
[Authorize(Roles = "Admin")]
public class AdminEventsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<AdminEventsController> _logger;

    public AdminEventsController(UEHStudentDbContext context, ILogger<AdminEventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var events = await _context.Events
            .AsNoTracking()
            .OrderByDescending(e => e.StartDateTime)
            .Select(e => new
            {
                e.EventId,
                e.Title,
                e.Description,
                e.Category,
                StartDateTime = e.StartDateTime.ToString("yyyy-MM-dd HH:mm"),
                EndDateTime = e.EndDateTime.ToString("yyyy-MM-dd HH:mm"),
                e.Location,
                RegistrationDeadline = e.RegistrationDeadline.HasValue ? e.RegistrationDeadline.Value.ToString("yyyy-MM-dd HH:mm") : null,
                e.MaxParticipants,
                RegisteredCount = _context.EventRegistrations.Count(r => r.EventId == e.EventId && r.Status == "Registered"),
                e.TargetAudience,
                e.IsActive
            })
            .ToListAsync();

        return Ok(events);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEventRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (request.StartDateTime >= request.EndDateTime)
        {
            return BadRequest(new { message = "Thời gian bắt đầu phải trước thời gian kết thúc." });
        }

        var evt = new Event
        {
            Title = request.Title.Trim(),
            Description = request.Description?.Trim(),
            Category = request.Category.Trim(),
            StartDateTime = request.StartDateTime,
            EndDateTime = request.EndDateTime,
            Location = request.Location?.Trim(),
            RegistrationDeadline = request.RegistrationDeadline,
            MaxParticipants = request.MaxParticipants,
            TargetAudience = request.TargetAudience?.Trim() ?? "Toàn trường",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Events.Add(evt);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Tạo sự kiện thành công.", eventId = evt.EventId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEventRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
            return NotFound(new { message = "Không tìm thấy sự kiện." });

        if (request.StartDateTime >= request.EndDateTime)
        {
            return BadRequest(new { message = "Thời gian bắt đầu phải trước thời gian kết thúc." });
        }

        evt.Title = request.Title.Trim();
        evt.Description = request.Description?.Trim();
        evt.Category = request.Category.Trim();
        evt.StartDateTime = request.StartDateTime;
        evt.EndDateTime = request.EndDateTime;
        evt.Location = request.Location?.Trim();
        evt.RegistrationDeadline = request.RegistrationDeadline;
        evt.MaxParticipants = request.MaxParticipants;
        evt.TargetAudience = request.TargetAudience?.Trim();
        evt.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cập nhật sự kiện thành công." });
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
            return NotFound(new { message = "Không tìm thấy sự kiện." });

        evt.IsActive = !evt.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = evt.IsActive ? "Đã kích hoạt lại sự kiện." : "Đã hủy kích hoạt sự kiện.", isActive = evt.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var evt = await _context.Events.FindAsync(id);
        if (evt == null)
            return NotFound(new { message = "Không tìm thấy sự kiện." });

        // Safe delete: if registrations exist, deactivate instead of physical delete
        var hasRegistrations = await _context.EventRegistrations.AnyAsync(r => r.EventId == id);
        if (hasRegistrations)
        {
            evt.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Sự kiện đã có sinh viên đăng ký tham gia nên đã được chuyển sang trạng thái 'Ngừng hiển thị' để đảm bảo an toàn dữ liệu." });
        }

        _context.Events.Remove(evt);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xóa sự kiện thành công." });
    }
}
