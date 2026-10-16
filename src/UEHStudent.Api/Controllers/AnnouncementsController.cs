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
public class AnnouncementsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<AnnouncementsController> _logger;

    public AnnouncementsController(UEHStudentDbContext context, ILogger<AnnouncementsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AnnouncementDto>>> GetAnnouncements(
        [FromQuery] string? category,
        [FromQuery] string? search,
        [FromQuery] string? priority)
    {
        var studentId = User.GetStudentId();

        try
        {
            var savedAnnouncementIds = new HashSet<int>();
            var savedPlannedDates = new Dictionary<int, (string Date, string? Note)>();

            if (studentId.HasValue)
            {
                var savedList = await _context.StudentSavedAnnouncements
                    .AsNoTracking()
                    .Where(s => s.StudentId == studentId.Value)
                    .Select(s => new { s.AnnouncementId, s.PlannedAt, s.PersonalNote })
                    .ToListAsync();

                foreach (var s in savedList)
                {
                    savedAnnouncementIds.Add(s.AnnouncementId);
                    savedPlannedDates[s.AnnouncementId] = (s.PlannedAt.ToString("yyyy-MM-dd"), s.PersonalNote);
                }
            }

            var query = _context.Announcements
                .AsNoTracking()
                .Where(a => a.IsActive);

            if (!string.IsNullOrEmpty(category) && category != "Tất cả")
            {
                query = query.Where(a => a.Category == category);
            }

            if (!string.IsNullOrEmpty(priority) && priority != "Tất cả")
            {
                query = query.Where(a => a.Priority == priority);
            }

            if (!string.IsNullOrEmpty(search))
            {
                var lower = search.ToLower();
                query = query.Where(a => a.Title.ToLower().Contains(lower) ||
                                         (a.Summary != null && a.Summary.ToLower().Contains(lower)) ||
                                         (a.Content != null && a.Content.ToLower().Contains(lower)) ||
                                         a.Issuer.ToLower().Contains(lower));
            }

            var list = await query
                .OrderByDescending(a => a.PublishedAt)
                .Select(a => new
                {
                    a.AnnouncementId,
                    a.Title,
                    a.Content,
                    a.Summary,
                    a.Issuer,
                    a.Category,
                    a.PublishedAt,
                    a.ExpiresAt,
                    a.DeadlineAt,
                    a.Priority,
                    a.IsActive
                })
                .ToListAsync();

            var result = list.Select(a => new AnnouncementDto
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Content = a.Content,
                Summary = a.Summary,
                Issuer = a.Issuer,
                Category = a.Category,
                PublishedAt = a.PublishedAt,
                ExpiresAt = a.ExpiresAt,
                DeadlineAt = a.DeadlineAt,
                Priority = a.Priority,
                IsActive = a.IsActive,
                IsSaved = savedAnnouncementIds.Contains(a.AnnouncementId),
                PlannedAt = savedPlannedDates.TryGetValue(a.AnnouncementId, out var val) ? val.Date : null,
                PersonalNote = savedPlannedDates.TryGetValue(a.AnnouncementId, out var noteVal) ? noteVal.Note : null
            }).ToList();

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcements");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải danh sách thông báo."
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnnouncementDto>> GetAnnouncementById(int id)
    {
        var studentId = User.GetStudentId();

        try
        {
            var a = await _context.Announcements
                .AsNoTracking()
                .FirstOrDefaultAsync(ann => ann.AnnouncementId == id && ann.IsActive);

            if (a == null)
            {
                return NotFound(new { message = $"Không tìm thấy thông báo ID = {id}" });
            }

            var saved = studentId.HasValue
                ? await _context.StudentSavedAnnouncements
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.StudentId == studentId.Value && s.AnnouncementId == id)
                : null;

            return Ok(new AnnouncementDto
            {
                AnnouncementId = a.AnnouncementId,
                Title = a.Title,
                Content = a.Content,
                Summary = a.Summary,
                Issuer = a.Issuer,
                Category = a.Category,
                PublishedAt = a.PublishedAt,
                ExpiresAt = a.ExpiresAt,
                DeadlineAt = a.DeadlineAt,
                Priority = a.Priority,
                IsActive = a.IsActive,
                IsSaved = saved != null,
                PlannedAt = saved?.PlannedAt.ToString("yyyy-MM-dd"),
                PersonalNote = saved?.PersonalNote
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving announcement {Id}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải chi tiết thông báo."
            });
        }
    }
}
