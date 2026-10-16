namespace UEHStudent.Api.DTOs;

public class ExamItemDto
{
    public int ExamScheduleId { get; set; }
    public int ClassSectionId { get; set; }
    public string ClassSectionCode { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string? GroupName { get; set; }
    public string ExamDate { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string DayOfWeekName { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string? EndTime { get; set; }
    public string? Room { get; set; }
    public string? Location { get; set; }
    public string ExamType { get; set; } = string.Empty;
    public string? ExamForm { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}

public class ExamListResponse
{
    public string? AcademicYear { get; set; }
    public string? Semester { get; set; }
    public List<ExamItemDto> Items { get; set; } = new();
}
