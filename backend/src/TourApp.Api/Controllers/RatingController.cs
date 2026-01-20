using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Ratings.DTOs;
using TourApp.Application.Ratings.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;

    public RatingController(IRatingService ratingService)
    {
        _ratingService = ratingService;
    }

    [Authorize(Roles = "Tourist")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<RatingDto>>> CreateRating([FromBody] CreateRatingRequest request)
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<RatingDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _ratingService.CreateRatingAsync(touristId.Value, request);

        if (!result.Success)
        {
            if (result.Error?.Code == "TOUR_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("tour/{tourId:guid}")]
    public async Task<ActionResult<ApiResponse<List<RatingDto>>>> GetRatingsForTour(Guid tourId)
    {
        var result = await _ratingService.GetRatingsForTourAsync(tourId);

        if (!result.Success)
        {
            if (result.Error?.Code == "TOUR_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("tour/{tourId:guid}/average")]
    public async Task<ActionResult<ApiResponse<double?>>> GetAverageRatingForTour(Guid tourId)
    {
        var result = await _ratingService.GetAverageRatingForTourAsync(tourId);

        if (!result.Success)
        {
            if (result.Error?.Code == "TOUR_NOT_FOUND")
            {
                return NotFound(result);
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
