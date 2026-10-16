using System.ComponentModel.DataAnnotations;

namespace UEHStudent.Api.DTOs;

public class CreateScheduleRuleRequest
{
    [Required]
    public int ClassSectionId { get; set; }

    [Required]
    [Range(1, 7, ErrorMessage = "DayOfWeek phai tu 1 (Thu Hai) den 7 (Chu Nhat).")]
    public byte DayOfWeek { get; set; }

    [Required]
    public string StartTime { get; set; } = string.Empty; // "07:30"

    [Required]
    public string EndTime { get; set; } = string.Empty; // "11:30"

    public string? Room { get; set; }
    public string? Instructor { get; set; }

    [Required]
    public string EffectiveStartDate { get; set; } = string.Empty; // "yyyy-MM-dd"

    [Required]
    public string EffectiveEndDate { get; set; } = string.Empty; // "yyyy-MM-dd"
}

public class UpdateScheduleRuleRequest : CreateScheduleRuleRequest
{
    public bool IsActive { get; set; } = true;
}

public class CreateExamScheduleRequest
{
    [Required]
    public int ClassSectionId { get; set; }

    [Required]
    public string ExamDate { get; set; } = string.Empty; // "yyyy-MM-dd"

    [Required]
    public string StartTime { get; set; } = string.Empty; // "08:00"

    public string? EndTime { get; set; } // "10:00"
    public string? Room { get; set; }
    public string? Location { get; set; }
    public string ExamType { get; set; } = "Thi cuoi hoc phan";
    public string? ExamForm { get; set; }
    public string? Notes { get; set; }
}

public class UpdateExamScheduleRequest : CreateExamScheduleRequest
{
    public bool IsActive { get; set; } = true;
}

public class CreateAnnouncementRequest
{
    [Required(ErrorMessage = "Tieu de khong duoc de trong.")]
    public string Title { get; set; } = string.Empty;

    public string? Content { get; set; }
    public string? Summary { get; set; }

    [Required(ErrorMessage = "Don vi/nguoi gui khong duoc de trong.")]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = "Thong bao chung";

    public DateTime? ExpiresAt { get; set; }
    public DateTime? DeadlineAt { get; set; }

    [Required]
    public string Priority { get; set; } = "Normal";

    public string TargetType { get; set; } = "All"; // All, Faculty, Major, Cohort
    public string? TargetValue { get; set; }
}

public class UpdateAnnouncementRequest : CreateAnnouncementRequest
{
    public bool IsActive { get; set; } = true;
}

public class CreateEventRequest
{
    [Required(ErrorMessage = "Tieu de su kien khong duoc de trong.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string Category { get; set; } = "Hoat dong";

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    public string? Location { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public int? MaxParticipants { get; set; }
    public string? TargetAudience { get; set; } = "Toan truong";
}

public class UpdateEventRequest : CreateEventRequest
{
    public bool IsActive { get; set; } = true;
}
