using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UEHStudent.Api.Data.Scaffolded;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/personal-tasks")]
[Authorize]
public class PersonalTasksController : ControllerBase
{
    private readonly UEHStudentDbContext _context;
    private readonly ILogger<PersonalTasksController> _logger;

    public PersonalTasksController(UEHStudentDbContext context, ILogger<PersonalTasksController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PersonalTaskDto>>> GetTasks(
        [FromQuery] string? date,
        [FromQuery] string? from,
        [FromQuery] string? to)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        try
        {
            var query = _context.PersonalTasks
                .AsNoTracking()
                .Where(t => t.StudentId == studentId.Value);

            if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var specificDate))
            {
                query = query.Where(t => t.PlannedDate == specificDate);
            }

            if (!string.IsNullOrEmpty(from) && DateOnly.TryParse(from, out var fromDate))
            {
                query = query.Where(t => t.PlannedDate >= fromDate);
            }

            if (!string.IsNullOrEmpty(to) && DateOnly.TryParse(to, out var toDate))
            {
                query = query.Where(t => t.PlannedDate <= toDate);
            }

            var tasks = await query
                .OrderBy(t => t.PlannedDate)
                .ThenBy(t => t.PlannedTime)
                .Select(t => new PersonalTaskDto
                {
                    TaskId = t.TaskId,
                    StudentId = t.StudentId,
                    Title = t.Title,
                    Description = t.Description,
                    PlannedDate = t.PlannedDate.ToString("yyyy-MM-dd"),
                    PlannedTime = t.PlannedTime.HasValue ? t.PlannedTime.Value.ToString(@"hh\:mm") : null,
                    DueDate = t.DueDate.HasValue ? t.DueDate.Value.ToString("yyyy-MM-dd") : null,
                    IsCompleted = t.IsCompleted,
                    CompletedAt = t.CompletedAt,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving personal tasks for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải danh sách việc cá nhân."
            });
        }
    }

    [HttpPost]
    public async Task<ActionResult<PersonalTaskDto>> CreateTask([FromBody] CreatePersonalTaskRequest request)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!DateOnly.TryParse(request.PlannedDate, out var plannedDate))
        {
            return BadRequest(new { message = "Ngày dự kiến không đúng định dạng (yyyy-MM-dd)." });
        }

        TimeOnly? plannedTime = null;
        if (!string.IsNullOrEmpty(request.PlannedTime) && TimeOnly.TryParse(request.PlannedTime, out var pt))
        {
            plannedTime = pt;
        }

        DateOnly? dueDate = null;
        if (!string.IsNullOrEmpty(request.DueDate) && DateOnly.TryParse(request.DueDate, out var dd))
        {
            dueDate = dd;
        }

        try
        {
            var task = new PersonalTask
            {
                StudentId = studentId.Value,
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                PlannedDate = plannedDate,
                PlannedTime = plannedTime,
                DueDate = dueDate,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.PersonalTasks.Add(task);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTasks), new { id = task.TaskId }, new PersonalTaskDto
            {
                TaskId = task.TaskId,
                StudentId = task.StudentId,
                Title = task.Title,
                Description = task.Description,
                PlannedDate = task.PlannedDate.ToString("yyyy-MM-dd"),
                PlannedTime = task.PlannedTime.HasValue ? task.PlannedTime.Value.ToString(@"hh\:mm") : null,
                DueDate = task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd") : null,
                IsCompleted = task.IsCompleted,
                CreatedAt = task.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating personal task for student {StudentId}", studentId);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tạo công việc cá nhân."
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdatePersonalTaskRequest request)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var task = await _context.PersonalTasks
            .FirstOrDefaultAsync(t => t.TaskId == id && t.StudentId == studentId.Value);

        if (task == null)
        {
            return NotFound(new { message = "Không tìm thấy công việc cá nhân cần sửa hoặc bạn không có quyền sửa công việc này." });
        }

        if (!DateOnly.TryParse(request.PlannedDate, out var plannedDate))
        {
            return BadRequest(new { message = "Ngày dự kiến không đúng định dạng." });
        }

        task.Title = request.Title.Trim();
        task.Description = request.Description?.Trim();
        task.PlannedDate = plannedDate;

        if (!string.IsNullOrEmpty(request.PlannedTime) && TimeOnly.TryParse(request.PlannedTime, out var pt))
        {
            task.PlannedTime = pt;
        }
        else
        {
            task.PlannedTime = null;
        }

        if (!string.IsNullOrEmpty(request.DueDate) && DateOnly.TryParse(request.DueDate, out var dd))
        {
            task.DueDate = dd;
        }
        else
        {
            task.DueDate = null;
        }

        if (request.IsCompleted && !task.IsCompleted)
        {
            task.IsCompleted = true;
            task.CompletedAt = DateTime.UtcNow;
        }
        else if (!request.IsCompleted && task.IsCompleted)
        {
            task.IsCompleted = false;
            task.CompletedAt = null;
        }

        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Cập nhật công việc cá nhân thành công.",
            task = new PersonalTaskDto
            {
                TaskId = task.TaskId,
                StudentId = task.StudentId,
                Title = task.Title,
                Description = task.Description,
                PlannedDate = task.PlannedDate.ToString("yyyy-MM-dd"),
                PlannedTime = task.PlannedTime.HasValue ? task.PlannedTime.Value.ToString(@"hh\:mm") : null,
                DueDate = task.DueDate.HasValue ? task.DueDate.Value.ToString("yyyy-MM-dd") : null,
                IsCompleted = task.IsCompleted,
                CompletedAt = task.CompletedAt,
                CreatedAt = task.CreatedAt
            }
        });
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTaskCompletion(int id)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        var task = await _context.PersonalTasks
            .FirstOrDefaultAsync(t => t.TaskId == id && t.StudentId == studentId.Value);

        if (task == null)
        {
            return NotFound(new { message = "Không tìm thấy công việc." });
        }

        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTime.UtcNow : null;
        task.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = task.IsCompleted ? "Đã đánh dấu hoàn thành." : "Đã hủy đánh dấu hoàn thành.",
            isCompleted = task.IsCompleted
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(int id)
    {
        var studentId = User.GetStudentId();
        if (!studentId.HasValue)
        {
            return Forbid();
        }

        var task = await _context.PersonalTasks
            .FirstOrDefaultAsync(t => t.TaskId == id && t.StudentId == studentId.Value);

        if (task == null)
        {
            return NotFound(new { message = "Không tìm thấy công việc cá nhân hoặc bạn không có quyền xóa." });
        }

        _context.PersonalTasks.Remove(task);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Xóa công việc cá nhân thành công." });
    }
}
