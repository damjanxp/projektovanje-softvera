using Microsoft.EntityFrameworkCore;
using TourApp.Application.Admin.DTOs;
using TourApp.Domain.Security;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Application.Admin.Services;

public class AdminService : IAdminService
{
    private readonly DbContext _dbContext;

    public AdminService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<List<BlockedUserDto>>> GetBlockedUsersAsync()
    {
        var trackersSet = _dbContext.Set<LoginAttemptTracker>();
        var touristsSet = _dbContext.Set<Tourist>();
        var systemUsersSet = _dbContext.Set<SystemUser>();

        var blockedTrackers = await trackersSet
            .Where(t => t.IsBlocked)
            .ToListAsync();

        var result = new List<BlockedUserDto>();

        foreach (var tracker in blockedTrackers)
        {
            var role = await DetermineUserRoleAsync(tracker.Username, touristsSet, systemUsersSet);

            result.Add(new BlockedUserDto
            {
                Username = tracker.Username,
                Role = role,
                BlockCount = tracker.BlockCount,
                BlockedAt = tracker.BlockedAt
            });
        }

        return ApiResponse<List<BlockedUserDto>>.Ok(result);
    }

    public async Task<ApiResponse<string>> UnblockUserAsync(string username)
    {
        var trackersSet = _dbContext.Set<LoginAttemptTracker>();
        var tracker = await trackersSet.FirstOrDefaultAsync(t => t.Username == username);

        if (tracker == null)
        {
            return ApiResponse<string>.Fail("USER_NOT_FOUND", $"No login tracker found for user '{username}'.");
        }

        if (!tracker.IsBlocked)
        {
            return ApiResponse<string>.Fail("NOT_BLOCKED", $"User '{username}' is not currently blocked.");
        }

        if (!tracker.CanBeUnblocked())
        {
            return ApiResponse<string>.Fail("CANNOT_UNBLOCK", 
                $"User '{username}' cannot be unblocked. User has been blocked {tracker.BlockCount} times (maximum is 3).");
        }

        tracker.Unblock();
        await _dbContext.SaveChangesAsync();

        return ApiResponse<string>.Ok($"User '{username}' has been unblocked successfully.");
    }

    private async Task<string> DetermineUserRoleAsync(
        string username, 
        DbSet<Tourist> touristsSet, 
        DbSet<SystemUser> systemUsersSet)
    {
        var tourist = await touristsSet.FirstOrDefaultAsync(t => t.Username == username);
        if (tourist != null)
        {
            return Role.Tourist.ToString();
        }

        var systemUser = await systemUsersSet.FirstOrDefaultAsync(u => u.Username == username);
        if (systemUser != null)
        {
            return systemUser.Role.ToString();
        }

        return "Unknown";
    }
}
