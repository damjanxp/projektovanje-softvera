using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TourApp.Application.Admin.DTOs;
using TourApp.Application.Admin.Services;
using TourApp.Shared;

namespace TourApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("blocked-users")]
    public async Task<ActionResult<ApiResponse<List<BlockedUserDto>>>> GetBlockedUsers()
    {
        var result = await _adminService.GetBlockedUsersAsync();
        return Ok(result);
    }

    [HttpPost("unblock/{username}")]
    public async Task<ActionResult<ApiResponse<string>>> UnblockUser(string username)
    {
        var result = await _adminService.UnblockUserAsync(username);

        if (!result.Success)
        {
            if (result.Error?.Code == "CANNOT_UNBLOCK")
            {
                return Conflict(result);
            }

            if (result.Error?.Code == "USER_NOT_FOUND" || result.Error?.Code == "NOT_BLOCKED")
            {
                return NotFound(result);
            }

            return BadRequest(result);
        }

        return Ok(result);
    }
}
