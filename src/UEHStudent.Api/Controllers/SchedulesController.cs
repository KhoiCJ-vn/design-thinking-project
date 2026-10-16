using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;
using UEHStudent.Api.Services;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly ILogger<SchedulesController> _logger;

    public SchedulesController(IScheduleService scheduleService, ILogger<SchedulesController> logger)
    {
        _scheduleService = scheduleService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<WeekScheduleResponse>> GetSchedule([FromQuery] string? date, [FromQuery] int? studentId)
    {
        var currentRole = User.GetRole();
        int targetStudentId;

        if (currentRole == "Admin" && studentId.HasValue)
        {
            targetStudentId = studentId.Value;
        }
        else
        {
            var loggedInStudentId = User.GetStudentId();
            if (!loggedInStudentId.HasValue)
            {
                return Forbid();
            }
            targetStudentId = loggedInStudentId.Value;
        }

        DateOnly selectedDate;
        if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
        {
            selectedDate = parsedDate;
        }
        else
        {
            selectedDate = new DateOnly(2026, 10, 16);
        }

        try
        {
            var response = await _scheduleService.GetStudentScheduleForWeekAsync(targetStudentId, selectedDate);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting schedule for student {StudentId} on date {Date}", targetStudentId, selectedDate);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải thời khóa biểu."
            });
        }
    }
}
