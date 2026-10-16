using System;
using System.Collections.Generic;

namespace UEHStudent.Api.Data.Scaffolded;

public partial class AppUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    public int? StudentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual Student? Student { get; set; }
}
