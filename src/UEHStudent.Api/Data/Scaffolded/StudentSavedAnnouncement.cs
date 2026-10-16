using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class StudentSavedAnnouncement
{
    public int SavedAnnouncementId { get; set; }

    public int StudentId { get; set; }

    public int AnnouncementId { get; set; }

    public DateOnly PlannedAt { get; set; }

    public string? PersonalNote { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Announcement Announcement { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
