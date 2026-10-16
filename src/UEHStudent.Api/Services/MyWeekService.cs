using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;

namespace UEHStudent.Api.Services;

public interface IMyWeekService
{
    Task<MyWeekResponse> GetMyWeekAsync(int studentId, DateOnly selectedDate);
}

public class MyWeekService : IMyWeekService
{
    private readonly UEHStudentDbContext _context;
    private readonly IScheduleService _scheduleService;

    public MyWeekService(UEHStudentDbContext context, IScheduleService scheduleService)
    {
        _context = context;
        _scheduleService = scheduleService;
    }

    public async Task<MyWeekResponse> GetMyWeekAsync(int studentId, DateOnly selectedDate)
    {
        var (weekStart, weekEnd) = DateTimeHelper.GetWeekRange(selectedDate);
        var weekStartDateTime = weekStart.ToDateTime(TimeOnly.MinValue);
        var weekEndDateTime = weekEnd.ToDateTime(TimeOnly.MaxValue);

        // 1. Lấy lớp học phần sinh viên đang theo học
        var enrolledSectionIds = await _context.StudentCourseEnrollments
            .AsNoTracking()
            .Where(e => e.StudentId == studentId && e.IsActive && e.Status == "Active")
            .Select(e => e.ClassSectionId)
            .ToListAsync();

        // 2. Lấy lịch học trong tuần (đã được mở rộng từ quy tắc lịch học)
        var scheduleItems = await _scheduleService.GetScheduleItemsForWeekAsync(studentId, weekStart, weekEnd);

        // 3. Lấy lịch thi trong tuần của các lớp sinh viên theo học
        var examEntities = await _context.ExamSchedules
            .AsNoTracking()
            .Include(e => e.ClassSection)
                .ThenInclude(cs => cs.Course)
            .Where(e => e.IsActive &&
                        enrolledSectionIds.Contains(e.ClassSectionId) &&
                        e.ExamDate >= weekStart &&
                        e.ExamDate <= weekEnd)
            .OrderBy(e => e.ExamDate)
            .ThenBy(e => e.StartTime)
            .ToListAsync();

        var examItems = examEntities.Select(e => new ExamItemDto
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
            EndTime = e.EndTime?.ToString(@"hh\:mm"),
            Room = e.Room ?? "Chưa xếp phòng",
            Location = e.Location ?? "Cơ sở Nguyễn Văn Linh",
            ExamType = e.ExamType,
            ExamForm = e.ExamForm ?? "Tự luận",
            Notes = e.Notes,
            IsActive = e.IsActive
        }).ToList();

        // 4. Lấy tất cả sự kiện trong tuần
        var allEventsInWeek = await _context.Events
            .AsNoTracking()
            .Where(ev => ev.IsActive &&
                         ev.StartDateTime <= weekEndDateTime &&
                         ev.EndDateTime >= weekStartDateTime)
            .OrderBy(ev => ev.StartDateTime)
            .ToListAsync();

        // Lấy danh sách ID sự kiện mà sinh viên đã đăng ký
        var registeredEventIds = await _context.EventRegistrations
            .AsNoTracking()
            .Where(er => er.StudentId == studentId && er.Status == "Registered")
            .Select(er => er.EventId)
            .ToListAsync();

        var uehEventsList = allEventsInWeek.Select(ev => new EventDto
        {
            EventId = ev.EventId,
            Title = ev.Title,
            Description = ev.Description,
            Category = ev.Category,
            StartDateTime = ev.StartDateTime,
            EndDateTime = ev.EndDateTime,
            Location = ev.Location,
            RegistrationDeadline = ev.RegistrationDeadline,
            MaxParticipants = ev.MaxParticipants,
            RegisteredCount = _context.EventRegistrations.Count(r => r.EventId == ev.EventId && r.Status == "Registered"),
            TargetAudience = ev.TargetAudience,
            IsRegistered = registeredEventIds.Contains(ev.EventId),
            CanRegister = !registeredEventIds.Contains(ev.EventId) &&
                          (!ev.RegistrationDeadline.HasValue || ev.RegistrationDeadline.Value >= DateTime.UtcNow),
            RegistrationStatus = registeredEventIds.Contains(ev.EventId) ? "Registered" : null
        }).ToList();

        // Sự kiện đã đăng ký nằm trong tuần
        var registeredEventsInWeek = uehEventsList
            .Where(ev => ev.IsRegistered)
            .ToList();

        // 5. Lấy việc cá nhân của sinh viên trong tuần
        var personalTasks = await _context.PersonalTasks
            .AsNoTracking()
            .Where(t => t.StudentId == studentId &&
                        t.PlannedDate >= weekStart &&
                        t.PlannedDate <= weekEnd)
            .OrderBy(t => t.PlannedDate)
            .ThenBy(t => t.PlannedTime)
            .Select(t => new PersonalTaskDto
            {
                TaskId = t.TaskId,
                StudentId = t.StudentId,
                Title = t.Title,
                Description = t.Description,
                PlannedDate = t.PlannedDate.ToString("yyyy-MM-dd"),
                PlannedTime = t.PlannedTime.HasValue ? t.PlannedTime.Value.ToString(@"hh\:mm") : null,
                DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd") : null,
                IsCompleted = t.IsCompleted,
                CompletedAt = t.CompletedAt,
                CreatedAt = t.CreatedAt
            })
            .ToListAsync();

        // 6. Lấy thông báo mà sinh viên đã lưu vào lịch và có ngày dự kiến trong tuần
        var savedAnnouncements = await _context.StudentSavedAnnouncements
            .AsNoTracking()
            .Include(sa => sa.Announcement)
            .Where(sa => sa.StudentId == studentId &&
                        sa.Announcement.IsActive &&
                        sa.PlannedAt >= weekStart &&
                        sa.PlannedAt <= weekEnd)
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

        // 7. Thông báo cần chú ý (đang hoạt động, ưu tiên High/Urgent hoặc có deadline chưa hết hạn)
        var allSavedIds = await _context.StudentSavedAnnouncements
            .AsNoTracking()
            .Where(sa => sa.StudentId == studentId)
            .Select(sa => sa.AnnouncementId)
            .ToListAsync();

        var relevantAnnouncements = await _context.Announcements
            .AsNoTracking()
            .Where(a => a.IsActive &&
                       (!a.ExpiresAt.HasValue || a.ExpiresAt.Value >= DateTime.UtcNow) &&
                       (a.Priority == "High" || a.Priority == "Urgent" || a.DeadlineAt.HasValue))
            .OrderByDescending(a => a.Priority == "Urgent")
            .ThenByDescending(a => a.Priority == "High")
            .ThenBy(a => a.DeadlineAt)
            .Take(6)
            .Select(a => new AnnouncementDto
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
                IsSaved = allSavedIds.Contains(a.AnnouncementId)
            })
            .ToListAsync();

        // 8. Tổng hợp Deadline Radar sắp tới (từ announcements có deadline, tasks có due date)
        var upcomingDeadlines = new List<DeadlineItemDto>();

        foreach (var ann in relevantAnnouncements.Where(a => a.DeadlineAt.HasValue))
        {
            var isUrgent = ann.DeadlineAt!.Value <= DateTime.UtcNow.AddDays(3);
            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"ann-{ann.AnnouncementId}",
                Title = ann.Title,
                Type = "Thông báo",
                Deadline = ann.DeadlineAt!.Value,
                FormattedDeadline = ann.DeadlineAt!.Value.ToString("dd/MM/yyyy HH:mm"),
                IsUrgent = isUrgent
            });
        }

        foreach (var task in personalTasks.Where(t => !string.IsNullOrEmpty(t.DueDate) && !t.IsCompleted))
        {
            if (DateOnly.TryParse(task.DueDate, out var dDate))
            {
                var dt = dDate.ToDateTime(new TimeOnly(23, 59, 59));
                upcomingDeadlines.Add(new DeadlineItemDto
                {
                    Id = $"task-{task.TaskId}",
                    Title = task.Title,
                    Type = "Việc cá nhân",
                    Deadline = dt,
                    FormattedDeadline = dDate.ToString("dd/MM/yyyy"),
                    IsUrgent = dDate <= selectedDate.AddDays(2)
                });
            }
        }

        upcomingDeadlines = upcomingDeadlines.OrderBy(d => d.Deadline).ToList();

        // 9. Xây dựng dòng thời gian theo 7 ngày trong tuần
        var today = DateOnly.FromDateTime(DateTime.UtcNow.AddHours(7)); // Múi giờ VN
        var dayGroups = new List<MyWeekDayGroupDto>();

        for (int i = 0; i < 7; i++)
        {
            var currentDay = weekStart.AddDays(i);
            var currentDayStr = currentDay.ToString("yyyy-MM-dd");
            var dayItems = new List<MyWeekTimelineItemDto>();

            // Lịch học của ngày này
            foreach (var sc in scheduleItems.Where(s => s.Date == currentDayStr))
            {
                dayItems.Add(new MyWeekTimelineItemDto
                {
                    Id = $"class-{sc.ScheduleRuleId}-{sc.Date}",
                    ItemType = "Class",
                    Title = sc.CourseName,
                    Subtitle = $"{sc.ClassSectionCode} • {sc.Instructor}",
                    Time = $"{sc.StartTime} - {sc.EndTime}",
                    Location = sc.Room,
                    Tag = sc.PeriodDescription,
                    TagColor = "blue",
                    RawData = sc
                });
            }

            // Lịch thi của ngày này
            foreach (var ex in examItems.Where(e => e.ExamDate == currentDayStr))
            {
                dayItems.Add(new MyWeekTimelineItemDto
                {
                    Id = $"exam-{ex.ExamScheduleId}",
                    ItemType = "Exam",
                    Title = $"[THI] {ex.CourseName}",
                    Subtitle = $"Hình thức: {ex.ExamForm} • {ex.Location}",
                    Time = $"{ex.StartTime}{(string.IsNullOrEmpty(ex.EndTime) ? "" : " - " + ex.EndTime)}",
                    Location = ex.Room,
                    Tag = "Kỳ thi",
                    TagColor = "red",
                    RawData = ex
                });
            }

            // Sự kiện đã đăng ký trong ngày này
            foreach (var ev in registeredEventsInWeek.Where(e => DateOnly.FromDateTime(e.StartDateTime) == currentDay))
            {
                dayItems.Add(new MyWeekTimelineItemDto
                {
                    Id = $"event-{ev.EventId}",
                    ItemType = "Event",
                    Title = $"[SỰ KIỆN] {ev.Title}",
                    Subtitle = ev.Location,
                    Time = $"{ev.StartDateTime:HH:mm} - {ev.EndDateTime:HH:mm}",
                    Location = ev.Location,
                    Tag = ev.Category,
                    TagColor = "purple",
                    RawData = ev
                });
            }

            // Việc cá nhân trong ngày này
            foreach (var pt in personalTasks.Where(t => t.PlannedDate == currentDayStr))
            {
                dayItems.Add(new MyWeekTimelineItemDto
                {
                    Id = $"task-{pt.TaskId}",
                    ItemType = "Task",
                    Title = pt.Title,
                    Subtitle = pt.Description,
                    Time = pt.PlannedTime ?? "Cả ngày",
                    Location = null,
                    Tag = pt.IsCompleted ? "Đã xong" : "Cần làm",
                    TagColor = pt.IsCompleted ? "green" : "orange",
                    RawData = pt
                });
            }

            // Thông báo được lưu vào ngày này
            foreach (var sa in savedAnnouncements.Where(s => s.PlannedAt == currentDayStr))
            {
                dayItems.Add(new MyWeekTimelineItemDto
                {
                    Id = $"saved-ann-{sa.AnnouncementId}",
                    ItemType = "SavedAnnouncement",
                    Title = $"[LƯU Ý] {sa.Title}",
                    Subtitle = sa.PersonalNote ?? sa.Summary,
                    Time = "Nhắc nhở theo dõi",
                    Location = null,
                    Tag = "Thông báo đã lưu",
                    TagColor = "teal",
                    RawData = sa
                });
            }

            // Sắp xếp các mục trong ngày theo giờ
            dayItems = dayItems.OrderBy(item => item.Time).ToList();

            dayGroups.Add(new MyWeekDayGroupDto
            {
                Date = currentDayStr,
                DayOfWeekName = DateTimeHelper.GetVietnameseDayOfWeekName(currentDay.DayOfWeek),
                IsToday = currentDay == today,
                Items = dayItems
            });
        }

        // 10. Tạo response
        return new MyWeekResponse
        {
            WeekStart = weekStart.ToString("yyyy-MM-dd"),
            WeekEnd = weekEnd.ToString("yyyy-MM-dd"),
            SelectedDate = selectedDate.ToString("yyyy-MM-dd"),
            FormattedWeekRange = $"{DateTimeHelper.FormatDateVietnamese(weekStart)} – {DateTimeHelper.FormatDateVietnamese(weekEnd)}",
            Summary = new MyWeekSummaryDto
            {
                TotalClasses = scheduleItems.Count,
                TotalExams = examItems.Count,
                TotalRegisteredEvents = registeredEventsInWeek.Count,
                PendingTasksCount = personalTasks.Count(t => !t.IsCompleted),
                ImportantAnnouncementsCount = relevantAnnouncements.Count
            },
            Days = dayGroups,
            Schedule = scheduleItems,
            Exams = examItems,
            RegisteredEvents = registeredEventsInWeek,
            PersonalTasks = personalTasks,
            SavedAnnouncements = savedAnnouncements,
            UpcomingDeadlines = upcomingDeadlines,
            RelevantAnnouncements = relevantAnnouncements,
            UehEvents = uehEventsList
        };
    }
}
