using Microsoft.EntityFrameworkCore;
using TourApp.Application.Ratings.Interfaces;
using TourApp.Domain.Ratings;

namespace TourApp.Infrastructure.Persistence.Repositories;

public class RatingRepository : IRatingRepository
{
    private readonly TourAppDbContext _context;

    public RatingRepository(TourAppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Rating rating)
    {
        await _context.Ratings.AddAsync(rating);
    }

    public async Task<Rating?> GetByIdAsync(Guid id)
    {
        return await _context.Ratings.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Rating?> GetByTouristAndTourAsync(Guid touristId, Guid tourId)
    {
        return await _context.Ratings
            .FirstOrDefaultAsync(r => r.TouristId == touristId && r.TourId == tourId);
    }

    public async Task<IEnumerable<Rating>> GetByTourAsync(Guid tourId)
    {
        return await _context.Ratings
            .Where(r => r.TourId == tourId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<double?> GetAverageRatingForTourAsync(Guid tourId)
    {
        var ratings = await _context.Ratings
            .Where(r => r.TourId == tourId)
            .Select(r => r.Score)
            .ToListAsync();

        if (ratings.Count == 0)
            return null;

        return ratings.Average();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
