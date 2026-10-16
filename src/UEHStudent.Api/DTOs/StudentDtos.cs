namespace UEHStudent.Api.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }
    public string StudentCode { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Faculty { get; set; }
    public string? Major { get; set; }
    public int? CohortYear { get; set; }
    public bool IsActive { get; set; }
}
