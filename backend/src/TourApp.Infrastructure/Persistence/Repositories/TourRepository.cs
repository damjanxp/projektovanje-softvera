using Microsoft.EntityFrameworkCore;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Tours;

namespace TourApp.Infrastructure.Persistence.Repositories;

public class TourRepository : ITourRepository
{
    private readonly TourAppDbContext _context;

    public TourRepository(TourAppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Tour tour)
    {
        await _context.Tours.AddAsync(tour);
    }

    public async Task<Tour?> GetByIdAsync(Guid id)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tour>> GetByGuideAsync(Guid guideId, bool sortAscending = true)
    {
        var query = _context.Tours
            .Include(t => t.KeyPoints)
            .Where(t => t.GuideId == guideId);

        query = sortAscending
            ? query.OrderBy(t => t.StartDate)
            : query.OrderByDescending(t => t.StartDate);

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Tour>> GetPublishedAsync(bool sortAscending = true)
    {
        var query = _context.Tours
            .Include(t => t.KeyPoints)
            .Where(t => t.Status == TourStatus.Published);

        query = sortAscending
            ? query.OrderBy(t => t.StartDate)
            : query.OrderByDescending(t => t.StartDate);

        return await query.ToListAsync();
    }

    public async Task<KeyPoint> AddKeyPointAsync(Tour tour, double latitude, double longitude, string name, string description, string imageUrl)
        {
            if (tour.Status == TourStatus.Published)
                throw new InvalidOperationException("Cannot add key points to a published tour.");

            if (tour.Status == TourStatus.Canceled)
                throw new InvalidOperationException("Cannot add key points to a canceled tour.");

            var order = tour.KeyPoints.Count;
            var keyPoint = new KeyPoint(Guid.NewGuid(), tour.Id, latitude, longitude, name, description, imageUrl, order);
        
            await _context.KeyPoints.AddAsync(keyPoint);
        
            return keyPoint;
        }

        public async Task<IEnumerable<Tour>> GetToursNeedingReplacementAsync(Guid excludeGuideId)
        {
            return await _context.Tours
                .Include(t => t.KeyPoints)
                .Where(t => t.NeedsReplacement 
                            && t.Status == TourStatus.Published 
                            && t.GuideId != excludeGuideId
                            && t.StartDate > DateTime.UtcNow)
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetToursStartingWithin24HoursNeedingReplacementAsync()
        {
            var now = DateTime.UtcNow;
            var in24Hours = now.AddHours(24);

            return await _context.Tours
                .Include(t => t.KeyPoints)
                .Where(t => t.NeedsReplacement 
                            && t.Status == TourStatus.Published 
                            && t.StartDate <= in24Hours 
                            && t.StartDate > now)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tour>> GetToursStartingIn48HoursAsync()
        {
            var now = DateTime.UtcNow;
            var in47Hours = now.AddHours(47);
            var in49Hours = now.AddHours(49);

            return await _context.Tours
                .Include(t => t.KeyPoints)
                .Where(t => t.Status == TourStatus.Published 
                            && !t.NeedsReplacement
                            && t.StartDate >= in47Hours 
                            && t.StartDate <= in49Hours)
                .ToListAsync();
        }

        public async Task<bool> HasTourOnDateAsync(Guid guideId, DateTime date)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            return await _context.Tours
                .AnyAsync(t => t.GuideId == guideId 
                              && t.Status == TourStatus.Published 
                              && t.StartDate >= startOfDay 
                              && t.StartDate < endOfDay);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
