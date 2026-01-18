namespace TourApp.Application.Admin.DTOs;

public class BlockedUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int BlockCount { get; set; }
    public DateTime? BlockedAt { get; set; }
}
