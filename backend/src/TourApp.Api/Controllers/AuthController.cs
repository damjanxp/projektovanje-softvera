using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Auth.DTOs;
using TourApp.Application.Auth.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register-tourist")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> RegisterTourist([FromBody] RegisterTouristRequest request)
    {
        var result = await _authService.RegisterTouristAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResponse>>> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);

        if (!result.Success)
        {
            if (result.Error?.Code == "ACCOUNT_LOCKED")
            {
                return StatusCode(423, result);
            }

            return Unauthorized(result);
        }

        return Ok(result);
    }
}
