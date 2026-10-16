using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class ExamSchedule
{
    public int ExamScheduleId { get; set; }

    public int ClassSectionId { get; set; }

    public DateOnly ExamDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Room { get; set; }

    public string? Location { get; set; }

    public string ExamType { get; set; } = null!;

    public string? ExamForm { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;
}
