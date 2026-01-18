using TourApp.Application.Admin.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Admin.Services;

public interface IAdminService
{
    Task<ApiResponse<List<BlockedUserDto>>> GetBlockedUsersAsync();
    Task<ApiResponse<string>> UnblockUserAsync(string username);
}
