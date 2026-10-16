using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;

namespace UEHStudent.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/announcements")]
[Authorize(Roles = "Admin")]
public class AdminAnnouncementsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<AdminAnnouncementsController> _logger;

    public AdminAnnouncementsController(UEHStudentDbContext context, ILogger<AdminAnnouncementsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var announcements = await _context.Announcements
            .AsNoTracking()
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();

        return Ok(announcements);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAnnouncementRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var announcement = new Announcement
        {
            Title = request.Title.Trim(),
            Content = request.Content?.Trim(),
            Summary = request.Summary?.Trim(),
            Issuer = request.Issuer.Trim(),
            Category = request.Category.Trim(),
            PublishedAt = DateTime.UtcNow,
            ExpiresAt = request.ExpiresAt,
            DeadlineAt = request.DeadlineAt,
            Priority = request.Priority.Trim(),
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();

        // Thêm target
        _context.AnnouncementTargets.Add(new AnnouncementTarget
        {
            AnnouncementId = announcement.AnnouncementId,
            TargetType = request.TargetType,
            TargetValue = request.TargetValue
        });
        await _context.SaveChangesAsync();

        return Ok(new { message = "Đăng thông báo thành công.", announcementId = announcement.AnnouncementId });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ann = await _context.Announcements.FindAsync(id);
        if (ann == null)
            return NotFound(new { message = "Không tìm thấy thông báo." });

        ann.Title = request.Title.Trim();
        ann.Content = request.Content?.Trim();
        ann.Summary = request.Summary?.Trim();
        ann.Issuer = request.Issuer.Trim();
        ann.Category = request.Category.Trim();
        ann.ExpiresAt = request.ExpiresAt;
        ann.DeadlineAt = request.DeadlineAt;
        ann.Priority = request.Priority.Trim();
        ann.IsActive = request.IsActive;

        await _context.SaveChangesAsync();

        return Ok(new { message = "Cập nhật thông báo thành công." });
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var ann = await _context.Announcements.FindAsync(id);
        if (ann == null)
            return NotFound(new { message = "Không tìm thấy thông báo." });

        ann.IsActive = !ann.IsActive;
        await _context.SaveChangesAsync();

        return Ok(new { message = ann.IsActive ? "Đã kích hoạt lại thông báo." : "Đã ẩn thông báo.", isActive = ann.IsActive });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ann = await _context.Announcements.FindAsync(id);
        if (ann == null)
            return NotFound(new { message = "Không tìm thấy thông báo." });

        // Xử lý an toàn: nếu có sinh viên đã lưu thông báo này vào lịch, ưu tiên đặt IsActive = false
        var hasSaved = await _context.StudentSavedAnnouncements.AnyAsync(s => s.AnnouncementId == id);
        if (hasSaved)
        {
            ann.IsActive = false;
            await _context.SaveChangesAsync();
            return Ok(new { message = "Thông báo đã có sinh viên lưu vào lịch cá nhân nên đã được chuyển sang trạng thái 'Ngừng hiển thị' để đảm bảo toàn vẹn dữ liệu." });
        }

        // Xóa targets trước
        var targets = await _context.AnnouncementTargets.Where(t => t.AnnouncementId == id).ToListAsync();
        _context.AnnouncementTargets.RemoveRange(targets);

        _context.Announcements.Remove(ann);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xóa thông báo thành công." });
    }
}
