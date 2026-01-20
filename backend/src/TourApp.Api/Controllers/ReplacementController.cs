using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Replacements.DTOs;
using TourApp.Application.Replacements.Services;
using TourApp.Application.Tours.DTOs;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Guide")]
public class ReplacementController : ControllerBase
{
    private readonly IReplacementService _replacementService;

    public ReplacementController(IReplacementService replacementService)
    {
        _replacementService = replacementService;
    }

    [HttpPost("request/{tourId:guid}")]
    public async Task<ActionResult<ApiResponse<TourDto>>> RequestReplacement(Guid tourId)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<TourDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _replacementService.RequestReplacementAsync(guideId.Value, tourId);

        if (!result.Success)
        {
            if (result.Error?.Code == "UNAUTHORIZED")
            {
                return Forbid();
            }
            if (result.Error?.Code == "TOUR_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("cancel/{tourId:guid}")]
    public async Task<ActionResult<ApiResponse<TourDto>>> CancelReplacementRequest(Guid tourId)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<TourDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _replacementService.CancelReplacementRequestAsync(guideId.Value, tourId);

        if (!result.Success)
        {
            if (result.Error?.Code == "UNAUTHORIZED")
            {
                return Forbid();
            }
            if (result.Error?.Code == "TOUR_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ReplacementTourDto>>>> GetReplacementTours()
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<List<ReplacementTourDto>>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _replacementService.GetReplacementToursAsync(guideId.Value);

        return Ok(result);
    }

    [HttpPost("take/{tourId:guid}")]
    public async Task<ActionResult<ApiResponse<TourDto>>> TakeOverTour(Guid tourId)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<TourDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _replacementService.TakeOverTourAsync(guideId.Value, tourId);

        if (!result.Success)
        {
            if (result.Error?.Code == "TOUR_NOT_FOUND")
            {
                return NotFound(result);
            }
            if (result.Error?.Code == "NOT_AVAILABLE")
            {
                return BadRequest(result);
            }
            if (result.Error?.Code == "SCHEDULE_CONFLICT")
            {
                return Conflict(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    private Guid? GetUserIdFromClaims()
    {
        var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (Guid.TryParse(subClaim, out var userId))
        {
            return userId;
        }

        return null;
    }
}
