namespace UEHStudent.Api.DTOs;

public class EventDto
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? Location { get; set; }
    public DateTime? RegistrationDeadline { get; set; }
    public int? MaxParticipants { get; set; }
    public int RegisteredCount { get; set; }
    public string? TargetAudience { get; set; }
    public bool IsRegistered { get; set; }
    public bool CanRegister { get; set; }
    public string? RegistrationStatus { get; set; }
}
