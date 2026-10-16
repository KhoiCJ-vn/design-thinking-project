using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<EventsController> _logger;

    public EventsController(UEHStudentDbContext context, ILogger<EventsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents(
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        var studentId = User.GetStudentId();

        try
        {
            var query = _context.Events
                .AsNoTracking()
                .Where(e => e.IsActive);

            if (!string.IsNullOrEmpty(from) && DateTime.TryParse(from, out var fromDt))
            {
                query = query.Where(e => e.EndDateTime >= fromDt);
            }

            if (!string.IsNullOrEmpty(to) && DateTime.TryParse(to, out var toDt))
            {
                query = query.Where(e => e.StartDateTime <= toDt);
            }

            var events = await query
                .OrderBy(e => e.StartDateTime)
                .ToListAsync();

            var registeredIds = new HashSet<int>();
            if (studentId.HasValue)
            {
                var regList = await _context.EventRegistrations
                    .AsNoTracking()
                    .Where(r => r.StudentId == studentId.Value && r.Status == "Registered")
                    .Select(r => r.EventId)
                    .ToListAsync();
                registeredIds = new HashSet<int>(regList);
            }

            var result = events.Select(e =>
            {
                var isRegistered = registeredIds.Contains(e.EventId);
                var isExpired = e.RegistrationDeadline.HasValue && e.RegistrationDeadline.Value < DateTime.UtcNow;
                var regCount = _context.EventRegistrations.Count(r => r.EventId == e.EventId && r.Status == "Registered");
                var isFull = e.MaxParticipants.HasValue && regCount >= e.MaxParticipants.Value;

                return new EventDto
                {
                    EventId = e.EventId,
                    Title = e.Title,
                    Description = e.Description,
                    Category = e.Category,
                    StartDateTime = e.StartDateTime,
                    EndDateTime = e.EndDateTime,
                    Location = e.Location,
                    RegistrationDeadline = e.RegistrationDeadline,
                    MaxParticipants = e.MaxParticipants,
                    RegisteredCount = regCount,
                    TargetAudience = e.TargetAudience,
                    IsRegistered = isRegistered,
                    CanRegister = !isRegistered && !isExpired && !isFull,
                    RegistrationStatus = isRegistered ? "Registered" : null
                };
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving events");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải danh sách sự kiện."
            });
        }
    }

    [HttpPost("{id}/register")]
    public async Task<IActionResult> RegisterForEvent(int id)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        try
        {
            var evt = await _context.Events.FirstOrDefaultAsync(e => e.EventId == id && e.IsActive);
            if (evt == null)
            {
                return NotFound(new { message = "Không tìm thấy sự kiện hoặc sự kiện đã ngừng hoạt động." });
            }

            // Kiểm tra hạn đăng ký
            if (evt.RegistrationDeadline.HasValue && evt.RegistrationDeadline.Value < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Sự kiện đã hết hạn đăng ký." });
            }

            // Kiểm tra số lượng người tham gia tối đa
            if (evt.MaxParticipants.HasValue)
            {
                var currentCount = await _context.EventRegistrations
                    .CountAsync(r => r.EventId == id && r.Status == "Registered");
                if (currentCount >= evt.MaxParticipants.Value)
                {
                    return BadRequest(new { message = "Sự kiện đã đủ số lượng người tham gia tối đa." });
                }
            }

            // Kiểm tra đăng ký trùng lặp
            var existing = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.StudentId == studentId.Value && r.EventId == id);

            if (existing != null)
            {
                if (existing.Status == "Registered")
                {
                    return Conflict(new { message = "Bạn đã đăng ký tham gia sự kiện này trước đó." });
                }
                // Nếu đã hủy trước đó, chuyển lại trạng thái Registered
                existing.Status = "Registered";
                existing.RegisteredAt = DateTime.UtcNow;
            }
            else
            {
                _context.EventRegistrations.Add(new EventRegistration
                {
                    StudentId = studentId.Value,
                    EventId = id,
                    RegisteredAt = DateTime.UtcNow,
                    Status = "Registered"
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đăng ký tham gia sự kiện thành công! Sự kiện đã được cập nhật vào lịch cá nhân của bạn.",
                eventId = id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering event {EventId} for student {StudentId}", id, studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Đã xảy ra lỗi trong quá trình đăng ký sự kiện."
            });
        }
    }

    [HttpDelete("{id}/registration")]
    public async Task<IActionResult> CancelRegistration(int id)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        try
        {
            var reg = await _context.EventRegistrations
                .FirstOrDefaultAsync(r => r.StudentId == studentId.Value && r.EventId == id);

            if (reg == null || reg.Status != "Registered")
            {
                return NotFound(new { message = "Bạn chưa đăng ký sự kiện này hoặc đã hủy trước đó." });
            }

            // Hủy đăng ký bằng cách xóa hoặc cập nhật Status
            _context.EventRegistrations.Remove(reg);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Hủy đăng ký sự kiện thành công. Sự kiện đã được gỡ khỏi lịch cá nhân của bạn.",
                eventId = id
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error canceling registration for event {EventId} for student {StudentId}", id, studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Đã xảy ra lỗi khi hủy đăng ký sự kiện."
            });
        }
    }
}
