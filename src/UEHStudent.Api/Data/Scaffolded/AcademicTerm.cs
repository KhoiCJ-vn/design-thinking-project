using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class AcademicTerm
{
    public int TermId { get; set; }

    public string AcademicYear { get; set; } = null!;

    public string Semester { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ClassSection> ClassSections { get; set; } = new List<ClassSection>();
}
