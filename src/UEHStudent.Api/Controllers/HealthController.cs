using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<HealthController> _logger;

    public HealthController(UEHStudentDbContext context, ILogger<HealthController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetHealth()
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync();
            if (!canConnect)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    status = "Unhealthy",
                    database = "Disconnected",
                    timestamp = DateTime.UtcNow
                });
            }

            var studentCount = await _context.Students.CountAsync();
            var termCount = await _context.AcademicTerms.CountAsync();

            return Ok(new
            {
                status = "Healthy",
                database = "Connected",
                server = "Microsoft SQL Server",
                stats = new
                {
                    students = studentCount,
                    academicTerms = termCount
                },
                timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                status = "Unhealthy",
                error = "Khong the ket noi den co so du lieu SQL Server.",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
