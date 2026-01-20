using TourApp.Domain.Problems;

namespace TourApp.Application.Problems.DTOs;

public class ProblemDto
{
    public Guid Id { get; set; }
    public Guid TouristId { get; set; }
    public Guid TourId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ProblemStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
