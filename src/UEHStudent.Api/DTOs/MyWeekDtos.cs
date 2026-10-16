namespace UEHStudent.Api.DTOs;

public class MyWeekSummaryDto
{
    public int TotalClasses { get; set; }
    public int TotalExams { get; set; }
    public int TotalRegisteredEvents { get; set; }
    public int PendingTasksCount { get; set; }
    public int ImportantAnnouncementsCount { get; set; }
}

public class MyWeekDayGroupDto
{
    public string Date { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string DayOfWeekName { get; set; } = string.Empty; // "Thứ Hai", v.v.
    public bool IsToday { get; set; }
    public List<MyWeekTimelineItemDto> Items { get; set; } = new();
}

public class MyWeekTimelineItemDto
{
    public string Id { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // "Class", "Exam", "Event", "Task", "SavedAnnouncement"
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Time { get; set; } // "07:30 - 11:30"
    public string? Location { get; set; }
    public string? Tag { get; set; }
    public string TagColor { get; set; } = "default"; // blue, orange, green, purple, red
    public object? RawData { get; set; }
}

public class DeadlineItemDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string Type { get; set; } = string.Empty; // "Việc cá nhân", "Lịch thi", "Sự kiện UEH", "Thông báo học vụ"
    public DateTime Deadline { get; set; }
    public string FormattedDeadline { get; set; } = string.Empty;
    public bool IsUrgent { get; set; }
    public string TagColor { get; set; } = "default";
}

public class MyWeekResponse
{
    public string WeekStart { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string WeekEnd { get; set; } = string.Empty;   // "yyyy-MM-dd"
    public string SelectedDate { get; set; } = string.Empty;
    public string FormattedWeekRange { get; set; } = string.Empty; // "12/10/2026 - 18/10/2026"
    public MyWeekSummaryDto Summary { get; set; } = new();
    public List<MyWeekDayGroupDto> Days { get; set; } = new();
    public List<ScheduleItemDto> Schedule { get; set; } = new();
    public List<ExamItemDto> Exams { get; set; } = new();
    public List<EventDto> RegisteredEvents { get; set; } = new();
    public List<PersonalTaskDto> PersonalTasks { get; set; } = new();
    public List<AnnouncementDto> SavedAnnouncements { get; set; } = new();
    public List<DeadlineItemDto> UpcomingDeadlines { get; set; } = new();
    public List<AnnouncementDto> RelevantAnnouncements { get; set; } = new();
    public List<EventDto> UehEvents { get; set; } = new();
}
