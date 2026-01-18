using Microsoft.EntityFrameworkCore;
using TourApp.Application.Auth.DTOs;
using TourApp.Domain.Security;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly DbContext _dbContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(DbContext dbContext, IPasswordHasher passwordHasher, IJwtTokenService jwtTokenService)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<ApiResponse<AuthResponse>> RegisterTouristAsync(RegisterTouristRequest request)
    {
        var touristsSet = _dbContext.Set<Tourist>();
        var systemUsersSet = _dbContext.Set<SystemUser>();

        var existingTourist = await touristsSet.FirstOrDefaultAsync(t => t.Username == request.Username);
        if (existingTourist != null)
        {
            return ApiResponse<AuthResponse>.Fail("USERNAME_EXISTS", "Username is already taken.");
        }

        var existingSystemUser = await systemUsersSet.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (existingSystemUser != null)
        {
            return ApiResponse<AuthResponse>.Fail("USERNAME_EXISTS", "Username is already taken.");
        }

        var existingEmail = await touristsSet.FirstOrDefaultAsync(t => t.Email == request.Email);
        if (existingEmail != null)
        {
            return ApiResponse<AuthResponse>.Fail("EMAIL_EXISTS", "Email is already registered.");
        }

        var passwordHash = _passwordHasher.Hash(request.Password);
        var touristId = Guid.NewGuid();

        var tourist = new Tourist(
            touristId,
            request.Username,
            request.Email,
            request.FirstName,
            request.LastName,
            passwordHash,
            request.WantsRecommendations,
            request.Interests
        );

        await touristsSet.AddAsync(tourist);
        await _dbContext.SaveChangesAsync();

        var token = _jwtTokenService.GenerateToken(tourist.Id, tourist.Username, Role.Tourist.ToString());

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            Role = Role.Tourist.ToString(),
            Username = tourist.Username
        });
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var trackersSet = _dbContext.Set<LoginAttemptTracker>();
        var tracker = await trackersSet.FirstOrDefaultAsync(t => t.Username == request.Username);

        if (tracker != null && tracker.IsBlocked)
        {
            return ApiResponse<AuthResponse>.Fail("ACCOUNT_LOCKED", 
                $"Account is locked due to too many failed login attempts. Block count: {tracker.BlockCount}");
        }

        var touristsSet = _dbContext.Set<Tourist>();
        var systemUsersSet = _dbContext.Set<SystemUser>();

        var tourist = await touristsSet.FirstOrDefaultAsync(t => t.Username == request.Username);
        if (tourist != null)
        {
            return await ProcessLoginAsync(
                request.Password,
                tourist.PasswordHash,
                tourist.Id,
                tourist.Username,
                Role.Tourist,
                tracker,
                trackersSet
            );
        }

        var systemUser = await systemUsersSet.FirstOrDefaultAsync(u => u.Username == request.Username);
        if (systemUser != null)
        {
            return await ProcessLoginAsync(
                request.Password,
                systemUser.PasswordHash,
                systemUser.Id,
                systemUser.Username,
                systemUser.Role,
                tracker,
                trackersSet
            );
        }

        await RecordFailedAttemptAsync(request.Username, tracker, trackersSet);
        return ApiResponse<AuthResponse>.Fail("INVALID_CREDENTIALS", "Invalid username or password.");
    }

    private async Task<ApiResponse<AuthResponse>> ProcessLoginAsync(
        string password,
        string passwordHash,
        Guid userId,
        string username,
        Role role,
        LoginAttemptTracker? tracker,
        DbSet<LoginAttemptTracker> trackersSet)
    {
        if (!_passwordHasher.Verify(password, passwordHash))
        {
            await RecordFailedAttemptAsync(username, tracker, trackersSet);

            var updatedTracker = await trackersSet.FirstOrDefaultAsync(t => t.Username == username);
            if (updatedTracker != null && updatedTracker.IsBlocked)
            {
                return ApiResponse<AuthResponse>.Fail("ACCOUNT_LOCKED", 
                    "Account has been locked due to too many failed login attempts.");
            }

            return ApiResponse<AuthResponse>.Fail("INVALID_CREDENTIALS", "Invalid username or password.");
        }

        if (tracker != null)
        {
            tracker.ResetFailedAttempts();
            await _dbContext.SaveChangesAsync();
        }

        var token = _jwtTokenService.GenerateToken(userId, username, role.ToString());

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            Token = token,
            Role = role.ToString(),
            Username = username
        });
    }

    private async Task RecordFailedAttemptAsync(
        string username, 
        LoginAttemptTracker? tracker, 
        DbSet<LoginAttemptTracker> trackersSet)
    {
        if (tracker == null)
        {
            tracker = new LoginAttemptTracker(Guid.NewGuid(), username);
            await trackersSet.AddAsync(tracker);
        }

        tracker.RecordFailedAttempt();
        await _dbContext.SaveChangesAsync();
    }
}
