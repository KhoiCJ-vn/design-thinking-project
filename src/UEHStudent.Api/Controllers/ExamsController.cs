using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;
using UEHStudent.Api.Services;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<ExamsController> _logger;

    public ExamsController(UEHStudentDbContext context, ILogger<ExamsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ExamListResponse>> GetExams(
        [FromQuery] int? termId,
        [FromQuery] string? from,
        [FromQuery] string? to,
        [FromQuery] int? studentId)
    {
        var currentRole = User.GetRole();
        int targetStudentId;

        if (currentRole == "Admin" && studentId.HasValue)
        {
            targetStudentId = studentId.Value;
        }
        else
        {
            var loggedInStudentId = User.GetStudentId();
            if (!loggedInStudentId.HasValue)
            {
                return Forbid();
            }
            targetStudentId = loggedInStudentId.Value;
        }

        try
        {
            // 1. Lay danh sach lop hoc phan sinh vien dang theo hoc
            var enrolledSectionIds = await _context.StudentCourseEnrollments
                .AsNoTracking()
                .Where(e => e.StudentId == targetStudentId && e.IsActive && e.Status == "Active")
                .Select(e => e.ClassSectionId)
                .ToListAsync();

            // 2. Query lich thi
            var query = _context.ExamSchedules
                .AsNoTracking()
                .Include(e => e.ClassSection)
                    .ThenInclude(cs => cs.Course)
                .Include(e => e.ClassSection)
                    .ThenInclude(cs => cs.Term)
                .Where(e => e.IsActive && enrolledSectionIds.Contains(e.ClassSectionId));

            if (termId.HasValue)
            {
                query = query.Where(e => e.ClassSection.TermId == termId.Value);
            }

            if (!string.IsNullOrEmpty(from) && DateOnly.TryParse(from, out var fromDate))
            {
                query = query.Where(e => e.ExamDate >= fromDate);
            }

            if (!string.IsNullOrEmpty(to) && DateOnly.TryParse(to, out var toDate))
            {
                query = query.Where(e => e.ExamDate <= toDate);
            }

            var exams = await query
                .OrderBy(e => e.ExamDate)
                .ThenBy(e => e.StartTime)
                .Select(e => new ExamItemDto
                {
                    ExamScheduleId = e.ExamScheduleId,
                    ClassSectionId = e.ClassSectionId,
                    ClassSectionCode = e.ClassSection.ClassSectionCode,
                    CourseCode = e.ClassSection.Course.CourseCode,
                    CourseName = e.ClassSection.Course.CourseName,
                    GroupName = e.ClassSection.GroupName,
                    ExamDate = e.ExamDate.ToString("yyyy-MM-dd"),
                    DayOfWeekName = DateTimeHelper.GetVietnameseDayOfWeekName(e.ExamDate.DayOfWeek),
                    StartTime = e.StartTime.ToString(@"hh\:mm"),
                    EndTime = e.EndTime.HasValue ? e.EndTime.Value.ToString(@"hh\:mm") : null,
                    Room = e.Room ?? "Chưa xếp",
                    Location = e.Location ?? "UEH Nguyễn Văn Linh",
                    ExamType = e.ExamType,
                    ExamForm = e.ExamForm ?? "Tự luận",
                    Notes = e.Notes,
                    IsActive = e.IsActive
                })
                .ToListAsync();

            var currentTerm = await _context.AcademicTerms
                .AsNoTracking()
                .Where(t => t.IsActive)
                .FirstOrDefaultAsync();

            return Ok(new ExamListResponse
            {
                AcademicYear = currentTerm?.AcademicYear ?? "2026-2027",
                Semester = currentTerm?.Semester ?? "Học kỳ cuối",
                Items = exams
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving exams for student {StudentId}", targetStudentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải lịch thi."
            });
        }
    }
}
