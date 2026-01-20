using Microsoft.EntityFrameworkCore;
using TourApp.Application.Problems.Interfaces;
using TourApp.Domain.Problems;

namespace TourApp.Infrastructure.Persistence.Repositories;

public class ProblemRepository : IProblemRepository
{
    private readonly TourAppDbContext _context;

    public ProblemRepository(TourAppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Problem problem)
    {
        await _context.Problems.AddAsync(problem);
    }

    public async Task<Problem?> GetByIdAsync(Guid id)
    {
        return await _context.Problems.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Problem?> GetByIdWithEventsAsync(Guid id)
    {
        return await _context.Problems
            .Include(p => p.Events)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Problem>> GetByTourAsync(Guid tourId)
    {
        return await _context.Problems
            .Where(p => p.TourId == tourId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Problem>> GetByTouristAsync(Guid touristId)
    {
        return await _context.Problems
            .Where(p => p.TouristId == touristId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Problem>> GetProblemsInReviewAsync()
    {
        return await _context.Problems
            .Where(p => p.Status == ProblemStatus.InReview)
            .OrderBy(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task AddEventAsync(ProblemStatusChangedEvent statusEvent)
    {
        await _context.ProblemEvents.AddAsync(statusEvent);
    }

    public async Task<IEnumerable<ProblemStatusChangedEvent>> GetEventsByProblemAsync(Guid problemId)
    {
        return await _context.ProblemEvents
            .Where(e => e.ProblemId == problemId)
            .OrderBy(e => e.ChangedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
