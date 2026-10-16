using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class Announcement
{
    public int AnnouncementId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? Summary { get; set; }

    public string Issuer { get; set; } = null!;

    public string Category { get; set; } = null!;

    public DateTime PublishedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public DateTime? DeadlineAt { get; set; }

    public string Priority { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AnnouncementTarget> AnnouncementTargets { get; set; } = new List<AnnouncementTarget>();

    public virtual ICollection<StudentSavedAnnouncement> StudentSavedAnnouncements { get; set; } = new List<StudentSavedAnnouncement>();
}
