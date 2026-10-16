using System.ComponentModel.DataAnnotations;

namespace UEHStudent.Api.DTOs;

public class AnnouncementDto
{
    public int AnnouncementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Content { get; set; }
    public string? Summary { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? DeadlineAt { get; set; }
    public string Priority { get; set; } = "Normal";
    public bool IsActive { get; set; }
    public bool IsSaved { get; set; }
    public string? PlannedAt { get; set; } // yyyy-MM-dd if saved by current student
    public string? PersonalNote { get; set; }
}

public class SaveAnnouncementRequest
{
    [Required]
    public int AnnouncementId { get; set; }

    [Required]
    public string PlannedAt { get; set; } = string.Empty; // "yyyy-MM-dd"

    public string? PersonalNote { get; set; }
}
