namespace TourApp.Application.Auth.Services;

public interface IJwtTokenService
{
    string GenerateToken(Guid userId, string username, string role);
}
