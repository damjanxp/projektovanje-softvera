using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TourApp.Application.Purchases.DTOs;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Tourist")]
public class UserController : ControllerBase
{
    private readonly DbContext _dbContext;

    public UserController(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("me/bonus-points")]
    public async Task<ActionResult<ApiResponse<BonusPointsDto>>> GetMyBonusPoints()
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<BonusPointsDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var tourist = await _dbContext.Set<Tourist>().FirstOrDefaultAsync(t => t.Id == touristId.Value);
        
        if (tourist == null)
        {
            return NotFound(ApiResponse<BonusPointsDto>.Fail("TOURIST_NOT_FOUND", "Tourist not found."));
        }

        var bonusPointsDto = new BonusPointsDto
        {
            Points = tourist.BonusPoints
        };

        return Ok(ApiResponse<BonusPointsDto>.Ok(bonusPointsDto));
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
