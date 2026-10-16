using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class AnnouncementTarget
{
    public int TargetId { get; set; }

    public int AnnouncementId { get; set; }

    public string TargetType { get; set; } = null!;

    public string? TargetValue { get; set; }

    public virtual Announcement Announcement { get; set; } = null!;
}
