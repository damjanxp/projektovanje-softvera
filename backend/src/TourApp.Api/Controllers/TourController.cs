using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Tours.DTOs;
using TourApp.Application.Tours.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TourController : ControllerBase
{
    private readonly ITourService _tourService;

    public TourController(ITourService tourService)
    {
        _tourService = tourService;
    }

    [Authorize(Roles = "Guide")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TourDto>>> CreateTour([FromBody] CreateTourRequest request)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<TourDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _tourService.CreateTourAsync(guideId.Value, request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Guide")]
    [HttpPost("{tourId:guid}/key-points")]
    public async Task<ActionResult<ApiResponse<KeyPointDto>>> AddKeyPoint(Guid tourId, [FromBody] AddKeyPointRequest request)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<KeyPointDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _tourService.AddKeyPointAsync(guideId.Value, tourId, request);

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

    [Authorize(Roles = "Guide")]
    [HttpPost("{tourId:guid}/publish")]
    public async Task<ActionResult<ApiResponse<TourDto>>> PublishTour(Guid tourId)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<TourDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _tourService.PublishTourAsync(guideId.Value, tourId);

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

    [Authorize(Roles = "Guide")]
    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<List<TourDto>>>> GetMyTours([FromQuery] string sort = "asc")
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<List<TourDto>>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var sortAscending = !string.Equals(sort, "desc", StringComparison.OrdinalIgnoreCase);
        var result = await _tourService.GetMyToursAsync(guideId.Value, sortAscending);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("published")]
    public async Task<ActionResult<ApiResponse<List<TourDto>>>> GetPublishedTours([FromQuery] string sort = "asc")
    {
        var sortAscending = !string.Equals(sort, "desc", StringComparison.OrdinalIgnoreCase);
        var result = await _tourService.GetPublishedToursAsync(sortAscending);

        return Ok(result);
    }

    [HttpGet("{tourId:guid}")]
    public async Task<ActionResult<ApiResponse<TourDto>>> GetTourById(Guid tourId)
    {
        var userId = GetUserIdFromClaims();
        var result = await _tourService.GetTourByIdAsync(tourId, userId);

        if (!result.Success)
        {
            return NotFound(result);
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
