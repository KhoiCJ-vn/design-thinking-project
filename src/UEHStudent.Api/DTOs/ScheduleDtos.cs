namespace UEHStudent.Api.DTOs;

public class ScheduleItemDto
{
    public int ScheduleRuleId { get; set; }
    public int ClassSectionId { get; set; }
    public string ClassSectionCode { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public byte DayOfWeek { get; set; } // 1=Mon, ..., 7=Sun
    public string DayOfWeekName { get; set; } = string.Empty; // "Thứ Hai", "Thứ Ba"...
    public string Date { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string StartTime { get; set; } = string.Empty; // "07:30"
    public string EndTime { get; set; } = string.Empty; // "11:30"
    public string? Room { get; set; }
    public string? Instructor { get; set; }
    public string PeriodDescription { get; set; } = string.Empty; // "Tiết 2-5"
    public int Credits { get; set; }
}

public class WeekScheduleResponse
{
    public string WeekStart { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string WeekEnd { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string SelectedDate { get; set; } = string.Empty;
    public List<ScheduleItemDto> Items { get; set; } = new();
}
