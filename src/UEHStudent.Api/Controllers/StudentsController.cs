using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<StudentsController> _logger;

    public StudentsController(UEHStudentDbContext context, ILogger<StudentsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudents()
    {
        try
        {
            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.IsActive)
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Faculty = s.Faculty,
                    Major = s.Major,
                    CohortYear = s.CohortYear,
                    IsActive = s.IsActive
                })
                .ToListAsync();

            return Ok(students);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving students from database");
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Loi khi truy van du lieu sinh vien tu he thong."
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StudentDto>> GetStudentById(int id)
    {
        try
        {
            var student = await _context.Students
                .AsNoTracking()
                .Where(s => s.StudentId == id && s.IsActive)
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    StudentCode = s.StudentCode,
                    FullName = s.FullName,
                    Faculty = s.Faculty,
                    Major = s.Major,
                    CohortYear = s.CohortYear,
                    IsActive = s.IsActive
                })
                .FirstOrDefaultAsync();

            if (student == null)
            {
                return NotFound(new { message = $"Khong tim thay sinh vien voi ID = {id}" });
            }

            return Ok(student);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving student {StudentId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Loi khi truy van thong tin sinh vien."
            });
        }
    }
}
