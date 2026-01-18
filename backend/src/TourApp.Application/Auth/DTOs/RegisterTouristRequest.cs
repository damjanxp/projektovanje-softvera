using TourApp.Domain.Users;

namespace TourApp.Application.Auth.DTOs;

public class RegisterTouristRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public List<Interest> Interests { get; set; } = new();
    public bool WantsRecommendations { get; set; }
}
