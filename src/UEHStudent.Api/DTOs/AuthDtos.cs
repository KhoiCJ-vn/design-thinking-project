using System.ComponentModel.DataAnnotations;

namespace UEHStudent.Api.DTOs;

public class LoginRequest
{
    [Required(ErrorMessage = "Ten dang nhap khong duoc de trong.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau khong duoc de trong.")]
    public string Password { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? StudentId { get; set; }
    public string? StudentCode { get; set; }
    public string? FullName { get; set; }
    public string? Faculty { get; set; }
    public string? Major { get; set; }
}

public class UserProfileDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int? StudentId { get; set; }
    public string? StudentCode { get; set; }
    public string? FullName { get; set; }
    public string? Faculty { get; set; }
    public string? Major { get; set; }
    public int? CohortYear { get; set; }
}
