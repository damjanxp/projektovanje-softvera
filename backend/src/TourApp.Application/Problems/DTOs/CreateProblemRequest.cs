namespace TourApp.Application.Problems.DTOs;

public class CreateProblemRequest
{
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
