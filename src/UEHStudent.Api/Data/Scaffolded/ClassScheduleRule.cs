using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class ClassScheduleRule
{
    public int ScheduleRuleId { get; set; }

    public int ClassSectionId { get; set; }

    public byte DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public string? Room { get; set; }

    public string? Instructor { get; set; }

    public DateOnly EffectiveStartDate { get; set; }

    public DateOnly EffectiveEndDate { get; set; }

    public string? WeekPattern { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;
}
