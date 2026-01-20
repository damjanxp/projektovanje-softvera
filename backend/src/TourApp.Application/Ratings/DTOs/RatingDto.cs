namespace TourApp.Application.Ratings.DTOs;

public class RatingDto
{
    public Guid Id { get; set; }
    public Guid TouristId { get; set; }
    public Guid TourId { get; set; }
    public int Score { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
