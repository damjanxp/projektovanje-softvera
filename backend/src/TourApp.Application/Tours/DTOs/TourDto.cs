using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tours.DTOs;

public class TourDto
{
    public Guid Id { get; set; }
    public Guid GuideId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public Interest Category { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public TourStatus Status { get; set; }
    public List<KeyPointDto> KeyPoints { get; set; } = new();
}
