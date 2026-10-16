using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UEHStudent.Api.DTOs;
using UEHStudent.Api.Security;
using UEHStudent.Api.Services;

namespace UEHStudent.Api.Controllers;

[ApiController]
[Route("api/my-week")]
[Authorize]
public class MyWeekController : ControllerBase
{
    private readonly IMyWeekService _myWeekService;
    private readonly ILogger<MyWeekController> _logger;

    public MyWeekController(IMyWeekService myWeekService, ILogger<MyWeekController> logger)
    {
        _myWeekService = myWeekService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<MyWeekResponse>> GetMyWeek([FromQuery] string? date, [FromQuery] int? studentId)
    {
        // 1. Xác định StudentId bảo mật từ Token
        var currentRole = User.GetRole();
        int targetStudentId;

        if (currentRole == "Admin" && studentId.HasValue)
        {
            // Cho phép Admin xem My Week của sinh viên cụ thể
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

        // 2. Xác định ngày được chọn (mặc định là ngày hôm nay, hoặc ngày truyền vào)
        DateOnly selectedDate;
        if (!string.IsNullOrEmpty(date) && DateOnly.TryParse(date, out var parsedDate))
        {
            selectedDate = parsedDate;
        }
        else
        {
            // Mặc định ngày 16/10/2026 nếu không truyền vào (khớp với dữ liệu mẫu học kỳ cuối 2026)
            selectedDate = new DateOnly(2026, 10, 16);
        }

        try
        {
            var result = await _myWeekService.GetMyWeekAsync(targetStudentId, selectedDate);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building My Week data for student {StudentId} on date {Date}", targetStudentId, selectedDate);
            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = "Không thể tải dữ liệu UEH My Week."
            });
        }
    }
}
