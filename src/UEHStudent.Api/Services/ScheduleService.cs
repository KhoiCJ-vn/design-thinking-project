using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;

namespace UEHStudent.Api.Services;

public interface IScheduleService
{
    Task<WeekScheduleResponse> GetStudentScheduleForWeekAsync(int studentId, DateOnly selectedDate);
    Task<List<ScheduleItemDto>> GetScheduleItemsForWeekAsync(int studentId, DateOnly weekStart, DateOnly weekEnd);
}

public class ScheduleService : IScheduleService
{
    private readonly UEHStudentDbContext _context;

    public ScheduleService(UEHStudentDbContext context)
    {
        _context = context;
    }

    public async Task<WeekScheduleResponse> GetStudentScheduleForWeekAsync(int studentId, DateOnly selectedDate)
    {
        var (weekStart, weekEnd) = DateTimeHelper.GetWeekRange(selectedDate);
        var items = await GetScheduleItemsForWeekAsync(studentId, weekStart, weekEnd);

        return new WeekScheduleResponse
        {
            WeekStart = weekStart.ToString("yyyy-MM-dd"),
            WeekEnd = weekEnd.ToString("yyyy-MM-dd"),
            SelectedDate = selectedDate.ToString("yyyy-MM-dd"),
            Items = items
        };
    }

    public async Task<List<ScheduleItemDto>> GetScheduleItemsForWeekAsync(int studentId, DateOnly weekStart, DateOnly weekEnd)
    {
        // 1. Lay danh sach ClassSectionId ma sinh vien dang hoc
        var enrolledSectionIds = await _context.StudentCourseEnrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId && e.IsActive && e.Status == "Active")
            .Select(e => e.ClassSectionId)
            .ToListAsync();

        if (!enrolledSectionIds.Any())
        {
            return new List<ScheduleItemDto>();
        }

        // 2. Lay cac quy tac lich hoc dang hoat dong cua cac lop nay
        var rules = await _context.ClassScheduleRules
            .AsNoTracking()
            .Include(r => r.ClassSection)
                .ThenInclude(cs => cs.Course)
            .Where(r => r.IsActive && enrolledSectionIds.Contains(r.ClassSectionId))
            .ToListAsync();

        var result = new List<ScheduleItemDto>();

        // 3. Mo rong cac quy tac thanh buoi hoc cu the trong tuan
        foreach (var rule in rules)
        {
            // Tuan bat dau tu Thu 2 (dayIndex = 0), DayOfWeek cua rule la 1 (Thu 2) den 7 (Chu Nhat)
            int dayOffset = rule.DayOfWeek - 1;
            var sessionDate = weekStart.AddDays(dayOffset);

            // Kiem tra ngay co nam trong khoang hieu luc cua quy tac khong
            if (sessionDate >= rule.EffectiveStartDate && sessionDate <= rule.EffectiveEndDate)
            {
                result.Add(new ScheduleItemDto
                {
                    ScheduleRuleId = rule.ScheduleRuleId,
                    ClassSectionId = rule.ClassSectionId,
                    ClassSectionCode = rule.ClassSection.ClassSectionCode,
                    CourseCode = rule.ClassSection.Course.CourseCode,
                    CourseName = rule.ClassSection.Course.CourseName,
                    GroupName = rule.ClassSection.GroupName,
                    DayOfWeek = rule.DayOfWeek,
                    DayOfWeekName = DateTimeHelper.GetVietnameseDayOfWeekName(rule.DayOfWeek),
                    Date = sessionDate.ToString("yyyy-MM-dd"),
                    StartTime = rule.StartTime.ToString(@"hh\:mm"),
                    EndTime = rule.EndTime.ToString(@"hh\:mm"),
                    Room = rule.Room ?? "Chua xep",
                    Instructor = rule.Instructor ?? "Chua phan cong",
                    PeriodDescription = GetPeriodDescription(rule.StartTime, rule.EndTime),
                    Credits = rule.ClassSection.Course.Credits
                });
            }
        }

        // Sap xep theo ngay va gio bat dau
        return result
            .OrderBy(i => i.Date)
            .ThenBy(i => i.StartTime)
            .ToList();
    }

    private static string GetPeriodDescription(TimeOnly start, TimeOnly end)
    {
        // Uoc tinh tiet hoc tieu chuan UEH (Tiet 1: 07:00, Tiet 2: 07:30 hoac 07:50, v.v.)
        if (start.Hour < 12)
        {
            return "Tiết 2 - 5";
        }
        else if (start.Hour < 17)
        {
            return "Tiết 8 - 12";
        }
        return "Buổi tối";
    }
}
