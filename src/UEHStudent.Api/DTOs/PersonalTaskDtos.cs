using System.ComponentModel.DataAnnotations;

namespace UEHStudent.Api.DTOs;

public class PersonalTaskDto
{
    public int TaskId { get; set; }
    public int StudentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PlannedDate { get; set; } = string.Empty; // "yyyy-MM-dd"
    public string? PlannedTime { get; set; } // "HH:mm"
    public string? DueDate { get; set; } // "yyyy-MM-dd"
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePersonalTaskRequest
{
    [Required(ErrorMessage = "Tieu de cong viec khong duoc de trong.")]
    [StringLength(200, ErrorMessage = "Tieu de khong duoc vuot qua 200 ky tu.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Ngay du kien khong duoc de trong.")]
    public string PlannedDate { get; set; } = string.Empty; // "yyyy-MM-dd"

    public string? PlannedTime { get; set; } // "HH:mm"

    public string? DueDate { get; set; } // "yyyy-MM-dd"
}

public class UpdatePersonalTaskRequest
{
    [Required(ErrorMessage = "Tieu de cong viec khong duoc de trong.")]
    [StringLength(200, ErrorMessage = "Tieu de khong duoc vuot qua 200 ky tu.")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Ngay du kien khong duoc de trong.")]
    public string PlannedDate { get; set; } = string.Empty; // "yyyy-MM-dd"

    public string? PlannedTime { get; set; } // "HH:mm"

    public string? DueDate { get; set; } // "yyyy-MM-dd"

    public bool IsCompleted { get; set; }
}
