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
                          (!ev.RegistrationDeadline.HasValue || ev.RegistrationDeadline.Value >= new DateTime(2026, 10, 16, 12, 0, 0)),
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

        // 7. Thông báo trường cần chú ý:
        // CHỈ hiển thị thông báo chung cấp trường (Học phí, Sinh hoạt công dân, Khám sức khỏe, Đào tạo chung)
        var allSavedIds = await _context.StudentSavedAnnouncements
            .AsNoTracking()
            .Where(sa => sa.StudentId == studentId)
            .Select(sa => sa.AnnouncementId)
            .ToListAsync();

        var relevantAnnouncements = await _context.Announcements
            .AsNoTracking()
            .Where(a => a.IsActive &&
                        (a.Category == "Tài chính sinh viên" ||
                         a.Category == "Hoạt động sinh viên" ||
                         a.Category == "Thông báo chung" ||
                         a.Category == "Đăng ký học phần" ||
                         a.Category == "Nộp chứng chỉ" ||
                         a.Priority == "Urgent"))
            .OrderByDescending(a => a.Priority == "Urgent")
            .ThenByDescending(a => a.Priority == "High")
            .ThenBy(a => a.DeadlineAt)
            .Take(4)
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
        // Phân loại nguồn rõ ràng:
        // - Bài tập LMS (Đồng bộ tự động từ hệ thống quản lý học tập LMS)
        // - Lịch thi kết thúc học phần
        // - Hạn cấp trường (Đóng học phí, sinh hoạt công dân, khám sức khỏe)
        // - Hạn đăng ký sự kiện UEH
        // - Việc cá nhân do sinh viên tự thêm
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
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(exam.ExamDate.DayOfWeek)}, {exam.ExamDate:dd/MM/yyyy} lúc {exam.StartTime:HH\\:mm}",
                IsUrgent = true,
                TagColor = "red"
            });
        }

        // 8.2. Việc học tập LMS & việc cá nhân sinh viên trong tuần này
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
            var tTime = task.DueDate.HasValue ? new TimeOnly(23, 59, 0) : (task.PlannedTime ?? new TimeOnly(23, 59, 0));
            var dt = dDate.ToDateTime(tTime);
            bool isLms = task.Title.StartsWith("[LMS]");
            string cleanTitle = isLms ? task.Title.Substring(5).Trim() : task.Title;
            string typeName = isLms ? "Bài tập LMS" : "Việc cá nhân";

            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"task-{task.TaskId}",
                Title = cleanTitle,
                Subtitle = !string.IsNullOrEmpty(task.Description) ? task.Description : (isLms ? "Đồng bộ tự động từ hệ thống LMS" : "Việc cá nhân tuần này"),
                Type = typeName,
                Deadline = dt,
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(dDate.DayOfWeek)}, {dDate:dd/MM/yyyy} lúc {tTime:HH\\:mm}",
                IsUrgent = isLms || dDate <= weekStart.AddDays(4),
                TagColor = isLms ? "orange" : "blue"
            });
        }

        // 8.3. Sự kiện UEH trong tuần: Đồng bộ tuyệt đối với danh sách Sự kiện UEH
        foreach (var ev in allEventsInWeek)
        {
            bool isRegistered = registeredEventIds.Contains(ev.EventId);

            if (isRegistered)
            {
                // Nếu sinh viên ĐÃ ĐĂNG KÝ sự kiện:
                // Hiển thị lịch tham gia sự kiện trong tuần
                upcomingDeadlines.Add(new DeadlineItemDto
                {
                    Id = $"event-attend-{ev.EventId}",
                    Title = $"Tham dự: {ev.Title}",
                    Subtitle = $"{ev.Location ?? "UEH"} • Bạn đã đăng ký tham gia",
                    Type = "Sự kiện UEH",
                    Deadline = ev.StartDateTime,
                    FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(ev.StartDateTime.DayOfWeek)}, {ev.StartDateTime:dd/MM/yyyy} lúc {ev.StartDateTime:HH\\:mm}",
                    IsUrgent = false,
                    TagColor = "purple"
                });
            }
            else
            {
                // Nếu sinh viên CHƯA ĐĂNG KÝ:
                // Hiển thị hạn đăng ký nếu hạn nằm trong tuần này và chưa qua ngày kết thúc
                if (ev.RegistrationDeadline.HasValue &&
                    ev.RegistrationDeadline.Value >= weekStartDateTime &&
                    ev.RegistrationDeadline.Value <= weekEndDateTime &&
                    ev.RegistrationDeadline.Value >= selectedDate.ToDateTime(TimeOnly.MinValue))
                {
                    upcomingDeadlines.Add(new DeadlineItemDto
                    {
                        Id = $"event-reg-{ev.EventId}",
                        Title = $"Hạn đăng ký: {ev.Title}",
                        Subtitle = $"{ev.Location ?? "UEH"} • Diễn ra: {ev.StartDateTime:dd/MM/yyyy lúc HH:mm}",
                        Type = "Hạn đăng ký sự kiện",
                        Deadline = ev.RegistrationDeadline.Value,
                        FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(ev.RegistrationDeadline.Value.DayOfWeek)}, {ev.RegistrationDeadline.Value:dd/MM/yyyy} lúc {ev.RegistrationDeadline.Value:HH\\:mm}",
                        IsUrgent = true,
                        TagColor = "purple"
                    });
                }
            }
        }

        // 8.4. Hạn chót các thông báo của trường rơi vào khoảng thời gian tuần này (Học phí, Sinh hoạt công dân...)
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
            bool isTuition = ann.Category == "Tài chính sinh viên" || ann.Title.ToLower().Contains("học phí");
            upcomingDeadlines.Add(new DeadlineItemDto
            {
                Id = $"ann-deadline-{ann.AnnouncementId}",
                Title = isTuition ? "Hạn nộp học phí Học kỳ cuối 2026-2027" : ann.Title,
                Subtitle = $"{ann.Category} • {ann.Issuer}",
                Type = isTuition ? "Hạn học phí" : "Hạn cấp trường",
                Deadline = ann.DeadlineAt!.Value,
                FormattedDeadline = $"{DateTimeHelper.GetVietnameseDayOfWeekName(ann.DeadlineAt.Value.DayOfWeek)}, {ann.DeadlineAt.Value:dd/MM/yyyy} lúc {ann.DeadlineAt.Value:HH\\:mm}",
                IsUrgent = true,
                TagColor = isTuition ? "red" : "teal"
            });
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
