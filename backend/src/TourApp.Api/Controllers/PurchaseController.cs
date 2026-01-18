using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Purchases.DTOs;
using TourApp.Application.Purchases.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Tourist")]
public class PurchaseController : ControllerBase
{
    private readonly IPurchaseService _purchaseService;

    public PurchaseController(IPurchaseService purchaseService)
    {
        _purchaseService = purchaseService;
    }

    [HttpPost("confirm")]
    public async Task<ActionResult<ApiResponse<PurchaseResultDto>>> ConfirmPurchase([FromQuery] bool useBonus = false)
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<PurchaseResultDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _purchaseService.ConfirmPurchaseAsync(touristId.Value, useBonus);

        if (!result.Success)
        {
            if (result.Error?.Code == "CART_EMPTY")
            {
                return BadRequest(result);
            }
            if (result.Error?.Code == "TOURIST_NOT_FOUND")
            {
                return NotFound(result);
            }
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<ActionResult<ApiResponse<List<PurchaseResultDto>>>> GetMyPurchases()
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<List<PurchaseResultDto>>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _purchaseService.GetMyPurchasesAsync(touristId.Value);

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
