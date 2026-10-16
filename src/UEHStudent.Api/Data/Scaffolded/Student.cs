using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class Student
{
    public int StudentId { get; set; }

    public string StudentCode { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Faculty { get; set; }

    public string? Major { get; set; }

    public int? CohortYear { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();

    public virtual ICollection<PersonalTask> PersonalTasks { get; set; } = new List<PersonalTask>();

    public virtual ICollection<StudentCourseEnrollment> StudentCourseEnrollments { get; set; } = new List<StudentCourseEnrollment>();

    public virtual ICollection<StudentSavedAnnouncement> StudentSavedAnnouncements { get; set; } = new List<StudentSavedAnnouncement>();
}
