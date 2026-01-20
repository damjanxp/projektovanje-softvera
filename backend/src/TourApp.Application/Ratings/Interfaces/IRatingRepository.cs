using TourApp.Domain.Ratings;

namespace TourApp.Application.Ratings.Interfaces;

public interface IRatingRepository
{
    Task AddAsync(Rating rating);
    Task<Rating?> GetByIdAsync(Guid id);
    Task<Rating?> GetByTouristAndTourAsync(Guid touristId, Guid tourId);
    Task<IEnumerable<Rating>> GetByTourAsync(Guid tourId);
    Task<double?> GetAverageRatingForTourAsync(Guid tourId);
    Task SaveChangesAsync();
}
