using TourApp.Application.Auth.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Auth.Services;

public interface IAuthService
{
    Task<ApiResponse<AuthResponse>> RegisterTouristAsync(RegisterTouristRequest request);
    Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
}
