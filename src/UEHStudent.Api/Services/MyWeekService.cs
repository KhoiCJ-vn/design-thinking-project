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

        // 7. Thông báo cần chú ý liên quan đến tuần này
        var allSavedIds = await _context.StudentSavedAnnouncements
            .AsNoTracking()
            .Where(sa => sa.StudentId == studentId)
            .Select(sa => sa.AnnouncementId)
            .ToListAsync();

        var relevantAnnouncements = await _context.Announcements
            .AsNoTracking()
            .Where(a => a.IsActive &&
                        a.PublishedAt <= weekEndDateTime &&
                        (!a.ExpiresAt.HasValue || a.ExpiresAt.Value >= weekStartDateTime))
            .OrderByDescending(a => a.DeadlineAt.HasValue && a.DeadlineAt.Value >= weekStartDateTime && a.DeadlineAt.Value <= weekEndDateTime)
            .ThenByDescending(a => a.Priority == "Urgent")
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

        // 8. Tổng hợp Hạn chót & Deadline đồng bộ 100% với khoảng thời gian UEH My Week [weekStart, weekEnd]
        // và liên quan trực tiếp đến lịch trình cá nhân của tuần này:
        // - Lịch thi các môn sinh viên học trong tuần
        // - Việc cá nhân & Bài tập môn học đến hạn trong tuần
        // - Hạn chót đăng ký các sự kiện UEH trong tuần
        // - Hạn chót thông báo học vụ / thủ tục trong tuần
        // - Nhắc nhở từ thông báo đã lưu vào lịch tuần này
        var upcomingDeadlines = new List<DeadlineItemDto>();

        // 8.1. Lịch thi cuối học phần trong tuần này của sinh viên
        foreach (var exam in examEntities)
        {
            var examDt = exam.ExamDate.ToDateTime(exam.StartTime);
            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"exam-{exam.ExamScheduleId}",
                Title = $"Thi: {exam.ClassSection.Course.CourseName}",
                Subtitle = $"{exam.ClassSection.ClassSectionCode} • {exam.Room ?? "Chưa xếp phòng"} ({exam.ExamForm ?? "Tự luận"})",
                Type = "Lịch thi",
                Deadline = examDt,
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(exam.ExamDate.DayOfWeek)}, {exam.ExamDate:dd/MM/yyyy} lúc {exam.StartTime:hh\\:mm}",
                IsUrgent = true,
                TagColor = "red"
            });
        }

        // 8.2. Việc cá nhân & bài tập môn học trong tuần này (chưa hoàn thành)
        var weekTasks = await _context.PersonalTasks
            .AsNoTracking()
            .Where(t => t.StudentId == studentId && !t.IsCompleted &&
                        ((t.DueDate.HasValue && t.DueDate.Value >= weekStart && t.DueDate.Value <= weekEnd) ||
                         (!t.DueDate.HasValue && t.PlannedDate >= weekStart && t.PlannedDate <= weekEnd)))
            .OrderBy(t => t.DueDate ?? t.PlannedDate)
            .ThenBy(t => t.PlannedTime)
            .ToListAsync();

        foreach (var task in weekTasks)
        {
            var dDate = task.DueDate ?? task.PlannedDate;
            var tTime = task.PlannedTime ?? new TimeOnly(23, 59, 59);
            var dt = dDate.ToDateTime(tTime);
            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"task-{task.TaskId}",
                Title = task.Title,
                Subtitle = !string.IsNullOrEmpty(task.Description) ? task.Description : "Việc cá nhân tuần này",
                Type = "Việc cá nhân",
                Deadline = dt,
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(dDate.DayOfWeek)}, {dDate:dd/MM/yyyy} {(task.PlannedTime.HasValue ? "lúc " + task.PlannedTime.Value.ToString(@"hh\:mm") : "23:59")}".Trim(),
                IsUrgent = dDate <= weekStart.AddDays(4),
                TagColor = "orange"
            });
        }

        // 8.3. Hạn chót đăng ký các sự kiện UEH trong tuần này
        var eventsWithDeadlineInWeek = await _context.Events
            .AsNoTracking()
            .Where(ev => ev.IsActive &&
                         ev.RegistrationDeadline.HasValue &&
                         ev.RegistrationDeadline.Value >= weekStartDateTime &&
                         ev.RegistrationDeadline.Value <= weekEndDateTime)
            .OrderBy(ev => ev.RegistrationDeadline)
            .ToListAsync();

        foreach (var ev in eventsWithDeadlineInWeek)
        {
            var isRegistered = registeredEventIds.Contains(ev.EventId);
            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"event-deadline-{ev.EventId}",
                Title = $"Hạn đăng ký: {ev.Title}",
                Subtitle = $"{ev.Location ?? "UEH"} • Diễn ra: {ev.StartDateTime:dd/MM/yyyy HH:mm} {(isRegistered ? "(Đã đăng ký)" : "")}".Trim(),
                Type = "Sự kiện UEH",
                Deadline = ev.RegistrationDeadline!.Value,
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(ev.RegistrationDeadline.Value.DayOfWeek)}, {ev.RegistrationDeadline.Value:dd/MM/yyyy HH:mm}",
                IsUrgent = true,
                TagColor = "purple"
            });
        }

        // 8.4. Hạn chót các thông báo của trường rơi vào khoảng thời gian tuần này
        var announcementsWithDeadlineInWeek = await _context.Announcements
            .AsNoTracking()
            .Where(a => a.IsActive &&
                        a.DeadlineAt.HasValue &&
                        a.DeadlineAt.Value >= weekStartDateTime &&
                        a.DeadlineAt.Value <= weekEndDateTime)
            .OrderBy(a => a.DeadlineAt)
            .ToListAsync();

        foreach (var ann in announcementsWithDeadlineInWeek)
        {
            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"ann-deadline-{ann.AnnouncementId}",
                Title = ann.Title,
                Subtitle = $"{ann.Category} • {ann.Issuer}",
                Type = "Thông báo học vụ",
                Deadline = ann.DeadlineAt!.Value,
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(ann.DeadlineAt.Value.DayOfWeek)}, {ann.DeadlineAt.Value:dd/MM/yyyy HH:mm}",
                IsUrgent = ann.Priority == "Urgent" || ann.Priority == "High",
                TagColor = "teal"
            });
        }

        // 8.5. Nhắc nhở theo dõi từ thông báo sinh viên đã lưu vào lịch trong tuần này
        foreach (var sa in savedAnnouncements)
        {
            if (DateOnly.TryParse(sa.PlannedAt, out var pDate))
            {
                var dt = pDate.ToDateTime(new TimeOnly(23, 59, 59));
                upcomingDeadlines.Add(new DeadlineItemDto
                {
                    Id = $"saved-ann-dl-{sa.AnnouncementId}",
                    Title = $"Nhắc nhở: {sa.Title}",
                    Subtitle = sa.PersonalNote ?? sa.Summary,
                    Type = "Thông báo đã lưu",
                    Deadline = dt,
                    FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(pDate.DayOfWeek)}, {pDate:dd/MM/yyyy}",
                    IsUrgent = false,
                    TagColor = "teal"
                });
            }
        }

        // Sắp xếp theo hạn chót gần nhất lên trước
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
