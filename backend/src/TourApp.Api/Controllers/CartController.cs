using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Cart.DTOs;
using TourApp.Application.Cart.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Tourist")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpPost("add")]
    public async Task<ActionResult<ApiResponse<CartDto>>> AddToCart([FromBody] AddToCartRequest request)
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<CartDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _cartService.AddToCartAsync(touristId.Value, request.TourId);

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

    [HttpPost("remove")]
    public async Task<ActionResult<ApiResponse<CartDto>>> RemoveFromCart([FromBody] RemoveFromCartRequest request)
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<CartDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _cartService.RemoveFromCartAsync(touristId.Value, request.TourId);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<CartDto>>> GetCart()
    {
        var touristId = GetUserIdFromClaims();
        if (touristId == null)
        {
            return Unauthorized(ApiResponse<CartDto>.Fail("UNAUTHORIZED", "User not authenticated."));
        }

        var result = await _cartService.GetCartAsync(touristId.Value);

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
