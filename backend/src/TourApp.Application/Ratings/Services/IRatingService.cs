using TourApp.Application.Ratings.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Ratings.Services;

public interface IRatingService
{
    Task<ApiResponse<RatingDto>> CreateRatingAsync(Guid touristId, CreateRatingRequest request);
    Task<ApiResponse<List<RatingDto>>> GetRatingsForTourAsync(Guid tourId);
    Task<ApiResponse<double?>> GetAverageRatingForTourAsync(Guid tourId);
}
