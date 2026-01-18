using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tours.DTOs;

public class CreateTourRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Difficulty Difficulty { get; set; }
    public Interest Category { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
}
