using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Cancellations.DTOs;
using TourApp.Application.Cancellations.Services;
using TourApp.Application.Notifications.DTOs;
using TourApp.Application.Notifications.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class BackgroundJobController : ControllerBase
{
    private readonly ICancellationService _cancellationService;
    private readonly IReminderService _reminderService;

    public BackgroundJobController(
        ICancellationService cancellationService,
        IReminderService reminderService)
    {
        _cancellationService = cancellationService;
        _reminderService = reminderService;
    }

    [HttpPost("cancel-unreplaced-tours")]
    public async Task<ActionResult<ApiResponse<CancellationResultDto>>> CancelUnreplacedTours()
    {
        var result = await _cancellationService.CancelToursWithoutReplacementAsync();

        return Ok(result);
    }

    [HttpPost("send-reminders")]
    public async Task<ActionResult<ApiResponse<ReminderResultDto>>> SendReminders()
    {
        var result = await _reminderService.SendTourRemindersAsync();

        return Ok(result);
    }

    [HttpPost("run-all")]
    public async Task<ActionResult<ApiResponse<BackgroundJobResultDto>>> RunAllJobs()
    {
        var cancellationResult = await _cancellationService.CancelToursWithoutReplacementAsync();
        var reminderResult = await _reminderService.SendTourRemindersAsync();

        var result = new BackgroundJobResultDto
        {
            CancellationResult = cancellationResult.Data,
            ReminderResult = reminderResult.Data
        };

        return Ok(ApiResponse<BackgroundJobResultDto>.Ok(result));
    }
}

public class BackgroundJobResultDto
{
    public CancellationResultDto? CancellationResult { get; set; }
    public ReminderResultDto? ReminderResult { get; set; }
}
