using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class ClassSection
{
    public int ClassSectionId { get; set; }

    public string ClassSectionCode { get; set; } = null!;

    public int CourseId { get; set; }

    public int TermId { get; set; }

    public string? GroupName { get; set; }

    public int MaxStudents { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<ClassScheduleRule> ClassScheduleRules { get; set; } = new List<ClassScheduleRule>();

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = new List<ExamSchedule>();

    public virtual ICollection<StudentCourseEnrollment> StudentCourseEnrollments { get; set; } = new List<StudentCourseEnrollment>();

    public virtual AcademicTerm Term { get; set; } = null!;
}
