using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class StudentCourseEnrollment
{
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    public int ClassSectionId { get; set; }

    public DateTime EnrolledAt { get; set; }

    public string Status { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual ClassSection ClassSection { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
