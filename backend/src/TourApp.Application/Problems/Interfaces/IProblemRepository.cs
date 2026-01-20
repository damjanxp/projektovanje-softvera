using TourApp.Domain.Problems;

namespace TourApp.Application.Problems.Interfaces;

public interface IProblemRepository
{
    Task AddAsync(Problem problem);
    Task<Problem?> GetByIdAsync(Guid id);
    Task<Problem?> GetByIdWithEventsAsync(Guid id);
    Task<IEnumerable<Problem>> GetByTourAsync(Guid tourId);
    Task<IEnumerable<Problem>> GetByTouristAsync(Guid touristId);
    Task<IEnumerable<Problem>> GetProblemsInReviewAsync();
    Task AddEventAsync(ProblemStatusChangedEvent statusEvent);
    Task<IEnumerable<ProblemStatusChangedEvent>> GetEventsByProblemAsync(Guid problemId);
    Task SaveChangesAsync();
}
