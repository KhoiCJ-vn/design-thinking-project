using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/saved-announcements")]
[Authorize]
public class SavedAnnouncementsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<SavedAnnouncementsController> _logger;

    public SavedAnnouncementsController(UEHStudentDbContext context, ILogger<SavedAnnouncementsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetSavedAnnouncements()
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        try
        {
            var saved = await _context.StudentSavedAnnouncements
                .AsNoTracking()
                .Include(sa => sa.Announcement)
                .Where(sa => sa.StudentId == studentId.Value && sa.Announcement.IsActive)
                .OrderBy(sa => sa.PlannedAt)
                .Select(sa => new AnnouncementDto
                {
                    AnnouncementId = sa.AnnouncementId,
                    Title = sa.Announcement.Title,
                    Content = sa.Announcement.Content,
                    Summary = sa.Announcement.Summary,
                    Issuer = sa.Announcement.Issuer,
                    Category = sa.Announcement.Category,
                    PublishedAt = sa.Announcement.PublishedAt,
                    ExpiresAt = sa.Announcement.ExpiresAt,
                    DeadlineAt = sa.Announcement.DeadlineAt,
                    Priority = sa.Announcement.Priority,
                    IsActive = sa.Announcement.IsActive,
                    IsSaved = true,
                    PlannedAt = sa.PlannedAt.ToString("yyyy-MM-dd"),
                    PersonalNote = sa.PersonalNote
                })
                .ToListAsync();

            return Ok(saved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving saved announcements for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải danh sách thông báo đã lưu."
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> SaveAnnouncement([FromBody] SaveAnnouncementRequest request)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!DateOnly.TryParse(request.PlannedAt, out var plannedDate))
        {
            return BadRequest(new { message = "Ngày dự kiến không đúng định dạng (yyyy-MM-dd)." });
        }

        try
        {
            var ann = await _context.Announcements
                .FirstOrDefaultAsync(a => a.AnnouncementId == request.AnnouncementId && a.IsActive);

            if (ann == null)
            {
                return NotFound(new { message = "Không tìm thấy thông báo hoặc thông báo đã hết hạn." });
            }

            var existing = await _context.StudentSavedAnnouncements
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId.Value && sa.AnnouncementId == request.AnnouncementId);

            if (existing != null)
            {
                // Cập nhật ngày dự kiến và ghi chú nếu đã lưu
                existing.PlannedAt = plannedDate;
                existing.PersonalNote = request.PersonalNote;
            }
            else
            {
                _context.StudentSavedAnnouncements.Add(new StudentSavedAnnouncement
                {
                    StudentId = studentId.Value,
                    AnnouncementId = request.AnnouncementId,
                    PlannedAt = plannedDate,
                    PersonalNote = request.PersonalNote,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đã thêm thông báo vào lịch cá nhân thành công.",
                announcementId = request.AnnouncementId,
                plannedAt = request.PlannedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving announcement for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể lưu thông báo vào lịch cá nhân."
            });
        }
    }

    [HttpDelete("{announcementId}")]
    public async Task<IActionResult> RemoveSavedAnnouncement(int announcementId)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        try
        {
            var record = await _context.StudentSavedAnnouncements
                .FirstOrDefaultAsync(sa => sa.StudentId == studentId.Value && sa.AnnouncementId == announcementId);

            if (record == null)
            {
                return NotFound(new { message = "Thông báo này chưa được lưu trong lịch cá nhân của bạn." });
            }

            _context.StudentSavedAnnouncements.Remove(record);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đã gỡ thông báo khỏi lịch cá nhân.",
                announcementId = announcementId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing saved announcement {AnnouncementId}", announcementId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể gỡ thông báo khỏi lịch cá nhân."
            });
        }
    }
}
