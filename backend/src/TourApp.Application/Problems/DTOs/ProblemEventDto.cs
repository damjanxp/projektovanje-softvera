using TourApp.Domain.Problems;

namespace TourApp.Application.Problems.DTOs;

public class ProblemEventDto
{
    public Guid Id { get; set; }
    public Guid ProblemId { get; set; }
    public ProblemStatus OldStatus { get; set; }
    public ProblemStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedByRole { get; set; } = string.Empty;
    public Guid ChangedByUserId { get; set; }
}
