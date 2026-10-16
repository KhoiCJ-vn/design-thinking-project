using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class PersonalTask
{
    public int TaskId { get; set; }

    public int StudentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public DateOnly PlannedDate { get; set; }

    public TimeOnly? PlannedTime { get; set; }

    public DateOnly? DueDate { get; set; }

    public bool IsCompleted { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Student Student { get; set; } = null!;
}
