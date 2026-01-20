using TourApp.Application.Purchases.Interfaces;
using TourApp.Application.Ratings.DTOs;
using TourApp.Application.Ratings.Interfaces;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Ratings;
using TourApp.Shared;

namespace TourApp.Application.Ratings.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ITourRepository _tourRepository;

    public RatingService(
        IRatingRepository ratingRepository,
        IPurchaseRepository purchaseRepository,
        ITourRepository tourRepository)
    {
        _ratingRepository = ratingRepository;
        _purchaseRepository = purchaseRepository;
        _tourRepository = tourRepository;
    }

    public async Task<ApiResponse<RatingDto>> CreateRatingAsync(Guid touristId, CreateRatingRequest request)
    {
        // Check if tour exists
        var tour = await _tourRepository.GetByIdAsync(request.TourId);
        if (tour == null)
        {
            return ApiResponse<RatingDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        // Check if tourist has purchased the tour
        var purchases = await _purchaseRepository.GetByTouristAsync(touristId);
        var hasPurchased = purchases
            .SelectMany(p => p.PurchasedTours)
            .Any(pt => pt.TourId == request.TourId);

        if (!hasPurchased)
        {
            return ApiResponse<RatingDto>.Fail("NOT_PURCHASED", "You can only rate tours you have purchased.");
        }

        // Check if tourist already rated this tour
        var existingRating = await _ratingRepository.GetByTouristAndTourAsync(touristId, request.TourId);
        if (existingRating != null)
        {
            return ApiResponse<RatingDto>.Fail("ALREADY_RATED", "You have already rated this tour.");
        }

        try
        {
            var rating = new Rating(
                Guid.NewGuid(),
                touristId,
                request.TourId,
                request.Score,
                request.Comment,
                tour.StartDate
            );

            await _ratingRepository.AddAsync(rating);
            await _ratingRepository.SaveChangesAsync();

            return ApiResponse<RatingDto>.Ok(MapToDto(rating));
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<RatingDto>.Fail("VALIDATION_ERROR", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<RatingDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<List<RatingDto>>> GetRatingsForTourAsync(Guid tourId)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<List<RatingDto>>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        var ratings = await _ratingRepository.GetByTourAsync(tourId);
        var dtos = ratings.Select(MapToDto).ToList();

        return ApiResponse<List<RatingDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<double?>> GetAverageRatingForTourAsync(Guid tourId)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<double?>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        var average = await _ratingRepository.GetAverageRatingForTourAsync(tourId);
        return ApiResponse<double?>.Ok(average);
    }

    private static RatingDto MapToDto(Rating rating)
    {
        return new RatingDto
        {
            Id = rating.Id,
            TouristId = rating.TouristId,
            TourId = rating.TourId,
            Score = rating.Score,
            Comment = rating.Comment,
            CreatedAt = rating.CreatedAt
        };
    }
}
