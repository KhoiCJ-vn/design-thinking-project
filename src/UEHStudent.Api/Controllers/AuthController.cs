using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UEHStudentDbContext context, IJwtService jwtService, ILogger<AuthController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.AppUsers
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null || !user.IsActive)
        {
            return Unauthorized(new { message = "Ten dang nhap hoac mat khau khong dung." });
        }

        bool isPasswordValid = PasswordHasher.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Ten dang nhap hoac mat khau khong dung." });
        }

        user.LastLoginAt = DateTime.UtcNow;

        // Neu mat khau trong database dang o dang text thuan, tu dong nang cap thanh BCrypt hash
        if (!user.PasswordHash.StartsWith("$2"))
        {
            user.PasswordHash = PasswordHasher.Hash(request.Password);
        }

        await _context.SaveChangesAsync();

        var token = _jwtService.GenerateToken(user, user.Student);

        return Ok(new LoginResponse
        {
            Token = token,
            UserId = user.UserId,
            Username = user.Username,
            Role = user.RoleName,
            StudentId = user.StudentId,
            StudentCode = user.Student?.StudentCode,
            FullName = user.Student?.FullName ?? (user.RoleName == "Admin" ? "Quan tri vien he thong" : user.Username),
            Faculty = user.Student?.Faculty,
            Major = user.Student?.Major
        });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserProfileDto>> GetCurrentUser()
    {
        var userId = User.GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized(new { message = "Khong xac dinh duoc danh tinh nguoi dung." });
        }

        var user = await _context.AppUsers
            .AsNoTracking()
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.UserId == userId.Value);

        if (user == null)
        {
            return NotFound(new { message = "Khong tim thay thong tin nguoi dung." });
        }

        return Ok(new UserProfileDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Role = user.RoleName,
            StudentId = user.StudentId,
            StudentCode = user.Student?.StudentCode,
            FullName = user.Student?.FullName ?? (user.RoleName == "Admin" ? "Quan tri vien he thong" : user.Username),
            Faculty = user.Student?.Faculty,
            Major = user.Student?.Major,
            CohortYear = user.Student?.CohortYear
        });
    }
}
