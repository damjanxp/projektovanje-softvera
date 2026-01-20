namespace TourApp.Application.Ratings.DTOs;

public class CreateRatingRequest
{
    public Guid TourId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
}
