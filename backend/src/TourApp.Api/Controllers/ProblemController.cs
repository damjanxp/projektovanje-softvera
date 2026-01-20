using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Problems.DTOs;
using TourApp.Application.Problems.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemController : ControllerBase
{
    private readonly IProblemService _problemService;

    public ProblemController(IProblemService problemService)
    {
        _problemService = problemService;
    }

    [Authorize(Roles = "Tourist")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProblemDto>>> CreateProblem([FromBody] CreateProblemRequest request)
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _problemService.CreateProblemAsync(touristId.Value, request);

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

    [Authorize(Roles = "Guide")]
    [HttpPost("{id:guid}/resolve")]
    public async Task<ActionResult<ApiResponse<ProblemDto>>> ResolveProblem(Guid id)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _problemService.ResolveProblemAsync(id, guideId.Value);

        if (!result.Success)
        {
            if (result.Error?.Code == "PROBLEM_NOT_FOUND")
            {
                return NotFound(result);
            }
            if (result.Error?.Code == "UNAUTHORIZED")
            {
                return Forbid();
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Guide")]
    [HttpPost("{id:guid}/review")]
    public async Task<ActionResult<ApiResponse<ProblemDto>>> SendToReview(Guid id)
    {
        var guideId = GetUserIdFromClaims();
        if (guideId == null)
        {
            return Unauthorized(ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _problemService.SendToReviewAsync(id, guideId.Value);

        if (!result.Success)
        {
            if (result.Error?.Code == "PROBLEM_NOT_FOUND")
            {
                return NotFound(result);
            }
            if (result.Error?.Code == "UNAUTHORIZED")
            {
                return Forbid();
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<ApiResponse<ProblemDto>>> RejectProblem(Guid id)
    {
        var adminId = GetUserIdFromClaims();
        if (adminId == null)
        {
            return Unauthorized(ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _problemService.RejectProblemAsync(id, adminId.Value);

        if (!result.Success)
        {
            if (result.Error?.Code == "PROBLEM_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("{id:guid}/reopen")]
    public async Task<ActionResult<ApiResponse<ProblemDto>>> ReopenProblem(Guid id)
    {
        var adminId = GetUserIdFromClaims();
        if (adminId == null)
        {
            return Unauthorized(ApiResponse<ProblemDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _problemService.ReopenProblemAsync(id, adminId.Value);

        if (!result.Success)
        {
            if (result.Error?.Code == "PROBLEM_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Tourist,Guide,Admin")]
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApiResponse<ProblemDto>>> GetProblemById(Guid id)
    {
        var result = await _problemService.GetProblemByIdAsync(id);

        if (!result.Success)
        {
            if (result.Error?.Code == "PROBLEM_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [Authorize(Roles = "Guide")]
    [HttpGet("tour/{tourId:guid}")]
    public async Task<ActionResult<ApiResponse<List<ProblemDto>>>> GetProblemsForTour(Guid tourId)
    {
        var result = await _problemService.GetProblemsForTourAsync(tourId);

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

    [Authorize(Roles = "Admin")]
    [HttpGet("in-review")]
    public async Task<ActionResult<ApiResponse<List<ProblemDto>>>> GetProblemsInReview()
    {
        var result = await _problemService.GetProblemsInReviewAsync();
        return Ok(result);
    }

    [Authorize(Roles = "Tourist,Guide,Admin")]
    [HttpGet("{id:guid}/events")]
    public async Task<ActionResult<ApiResponse<List<ProblemEventDto>>>> GetProblemEvents(Guid id)
    {
        var result = await _problemService.GetProblemEventsAsync(id);

        if (!result.Success)
        {
            if (result.Error?.Code == "PROBLEM_NOT_FOUND")
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
