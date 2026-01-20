using TourApp.Domain.Tours;

namespace TourApp.Application.Tours.Interfaces;

public interface ITourRepository
{
    Task AddAsync(Tour tour);
    Task<Tour?> GetByIdAsync(Guid id);
    Task<IEnumerable<Tour>> GetByGuideAsync(Guid guideId, bool sortAscending = true);
    Task<IEnumerable<Tour>> GetPublishedAsync(bool sortAscending = true);
    Task<KeyPoint> AddKeyPointAsync(Tour tour, double latitude, double longitude, string name, string description, string imageUrl);
    Task<IEnumerable<Tour>> GetToursNeedingReplacementAsync(Guid excludeGuideId);
    Task<IEnumerable<Tour>> GetToursStartingWithin24HoursNeedingReplacementAsync();
    Task<IEnumerable<Tour>> GetToursStartingIn48HoursAsync();
    Task<bool> HasTourOnDateAsync(Guid guideId, DateTime date);
    Task SaveChangesAsync();
}
