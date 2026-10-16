using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class UEHStudentDbContext : DbContext
{
    public UEHStudentDbContext(DbContextOptions<UEHStudentDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AcademicTerm> AcademicTerms { get; set; }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<AnnouncementTarget> AnnouncementTargets { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<ClassScheduleRule> ClassScheduleRules { get; set; }

    public virtual DbSet<ClassSection> ClassSections { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventRegistration> EventRegistrations { get; set; }

    public virtual DbSet<ExamSchedule> ExamSchedules { get; set; }

    public virtual DbSet<PersonalTask> PersonalTasks { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentCourseEnrollment> StudentCourseEnrollments { get; set; }

    public virtual DbSet<StudentSavedAnnouncement> StudentSavedAnnouncements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AcademicTerm>(entity =>
        {
            entity.HasKey(e => e.TermId);

            entity.HasIndex(e => new { e.AcademicYear, e.Semester }, "UQ_AcademicTerms_YearSemester").IsUnique();

            entity.Property(e => e.AcademicYear).HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Semester).HasMaxLength(30);
        });

        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasIndex(e => new { e.PublishedAt, e.IsActive }, "IX_Ann_PublishedActive").IsDescending(true, false);

            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasDefaultValue("Thong bao chung");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Issuer).HasMaxLength(100);
            entity.Property(e => e.Priority)
                .HasMaxLength(20)
                .HasDefaultValue("Normal");
            entity.Property(e => e.PublishedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Summary).HasMaxLength(500);
            entity.Property(e => e.Title).HasMaxLength(500);
        });

        modelBuilder.Entity<AnnouncementTarget>(entity =>
        {
            entity.HasKey(e => e.TargetId);

            entity.HasIndex(e => e.AnnouncementId, "IX_AT_AnnouncementId");

            entity.Property(e => e.TargetType)
                .HasMaxLength(30)
                .HasDefaultValue("All");
            entity.Property(e => e.TargetValue).HasMaxLength(100);

            entity.HasOne(d => d.Announcement).WithMany(p => p.AnnouncementTargets)
                .HasForeignKey(d => d.AnnouncementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AT_Announcements");
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.HasIndex(e => e.StudentId, "IX_AppUsers_StudentId").HasFilter("([StudentId] IS NOT NULL)");

            entity.HasIndex(e => e.Username, "UQ_AppUsers_Username").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.RoleName)
                .HasMaxLength(20)
                .HasDefaultValue("Student");
            entity.Property(e => e.Username).HasMaxLength(50);

            entity.HasOne(d => d.Student).WithMany(p => p.AppUsers)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_AppUsers_Students");
        });

        modelBuilder.Entity<ClassScheduleRule>(entity =>
        {
            entity.HasKey(e => e.ScheduleRuleId);

            entity.HasIndex(e => new { e.ClassSectionId, e.DayOfWeek }, "IX_CSR_ClassSection_Day");

            entity.HasIndex(e => new { e.EffectiveStartDate, e.EffectiveEndDate }, "IX_CSR_EffectiveDates");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.Instructor).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Room).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasPrecision(0);
            entity.Property(e => e.WeekPattern)
                .HasMaxLength(20)
                .HasDefaultValue("Every");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.ClassScheduleRules)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CSR_ClassSections");
        });

        modelBuilder.Entity<ClassSection>(entity =>
        {
            entity.HasIndex(e => e.ClassSectionCode, "UQ_ClassSections_Code").IsUnique();

            entity.Property(e => e.ClassSectionCode).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.GroupName).HasMaxLength(20);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.MaxStudents).HasDefaultValue(50);

            entity.HasOne(d => d.Course).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSections_Courses");

            entity.HasOne(d => d.Term).WithMany(p => p.ClassSections)
                .HasForeignKey(d => d.TermId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClassSections_AcademicTerms");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasIndex(e => e.CourseCode, "UQ_Courses_CourseCode").IsUnique();

            entity.Property(e => e.CourseCode).HasMaxLength(30);
            entity.Property(e => e.CourseName).HasMaxLength(200);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Credits).HasDefaultValue(3);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasIndex(e => new { e.StartDateTime, e.EndDateTime }, "IX_Events_DateTime");

            entity.Property(e => e.Category)
                .HasMaxLength(50)
                .HasDefaultValue("Hoat dong");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.TargetAudience)
                .HasMaxLength(100)
                .HasDefaultValue("Toan truong");
            entity.Property(e => e.Title).HasMaxLength(300);
        });

        modelBuilder.Entity<EventRegistration>(entity =>
        {
            entity.HasKey(e => e.RegistrationId);

            entity.HasIndex(e => e.StudentId, "IX_ER_StudentId");

            entity.HasIndex(e => new { e.StudentId, e.EventId }, "UQ_EventRegistrations").IsUnique();

            entity.Property(e => e.RegisteredAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Registered");

            entity.HasOne(d => d.Event).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ER_Events");

            entity.HasOne(d => d.Student).WithMany(p => p.EventRegistrations)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ER_Students");
        });

        modelBuilder.Entity<ExamSchedule>(entity =>
        {
            entity.HasIndex(e => e.ExamDate, "IX_ES_ExamDate");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.EndTime).HasPrecision(0);
            entity.Property(e => e.ExamForm).HasMaxLength(100);
            entity.Property(e => e.ExamType)
                .HasMaxLength(50)
                .HasDefaultValue("Thi cuoi hoc phan");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Room).HasMaxLength(50);
            entity.Property(e => e.StartTime).HasPrecision(0);

            entity.HasOne(d => d.ClassSection).WithMany(p => p.ExamSchedules)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ES_ClassSections");
        });

        modelBuilder.Entity<PersonalTask>(entity =>
        {
            entity.HasKey(e => e.TaskId);

            entity.HasIndex(e => new { e.StudentId, e.PlannedDate }, "IX_PT_StudentPlannedDate");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.PlannedTime).HasPrecision(0);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Student).WithMany(p => p.PersonalTasks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PT_Students");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.StudentCode, "UQ_Students_StudentCode").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Faculty).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(100);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Major).HasMaxLength(100);
            entity.Property(e => e.StudentCode).HasMaxLength(20);
        });

        modelBuilder.Entity<StudentCourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId);

            entity.HasIndex(e => e.StudentId, "IX_SCE_StudentId");

            entity.HasIndex(e => new { e.StudentId, e.ClassSectionId }, "UQ_StudentCourseEnrollments").IsUnique();

            entity.Property(e => e.EnrolledAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("Active");

            entity.HasOne(d => d.ClassSection).WithMany(p => p.StudentCourseEnrollments)
                .HasForeignKey(d => d.ClassSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SCE_ClassSections");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentCourseEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SCE_Students");
        });

        modelBuilder.Entity<StudentSavedAnnouncement>(entity =>
        {
            entity.HasKey(e => e.SavedAnnouncementId);

            entity.HasIndex(e => new { e.StudentId, e.PlannedAt }, "IX_SSA_StudentPlanned");

            entity.HasIndex(e => new { e.StudentId, e.AnnouncementId }, "UQ_SSA_StudentAnnouncement").IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.PersonalNote).HasMaxLength(300);

            entity.HasOne(d => d.Announcement).WithMany(p => p.StudentSavedAnnouncements)
                .HasForeignKey(d => d.AnnouncementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SSA_Announcements");

            entity.HasOne(d => d.Student).WithMany(p => p.StudentSavedAnnouncements)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SSA_Students");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
