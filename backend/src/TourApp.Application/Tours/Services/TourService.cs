using TourApp.Application.Tours.DTOs;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Tours;
using TourApp.Shared;

namespace TourApp.Application.Tours.Services;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;

    public TourService(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public async Task<ApiResponse<TourDto>> CreateTourAsync(Guid guideId, CreateTourRequest request)
    {
        try
        {
            var tour = new Tour(
                Guid.NewGuid(),
                guideId,
                request.Name,
                request.Description,
                request.Difficulty,
                request.Category,
                request.Price,
                request.StartDate
            );

            await _tourRepository.AddAsync(tour);
            await _tourRepository.SaveChangesAsync();

            return ApiResponse<TourDto>.Ok(MapToDto(tour));
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<TourDto>.Fail("VALIDATION_ERROR", ex.Message);
        }
    }

    public async Task<ApiResponse<KeyPointDto>> AddKeyPointAsync(Guid guideId, Guid tourId, AddKeyPointRequest request)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<KeyPointDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        if (tour.GuideId != guideId)
        {
            return ApiResponse<KeyPointDto>.Fail("UNAUTHORIZED", "You are not the owner of this tour.");
        }

        try
        {
            var keyPoint = await _tourRepository.AddKeyPointAsync(
                tour,
                request.Latitude,
                request.Longitude,
                request.Name,
                request.Description,
                request.ImageUrl
            );

            await _tourRepository.SaveChangesAsync();

            return ApiResponse<KeyPointDto>.Ok(MapKeyPointToDto(keyPoint));
        }
        catch (ArgumentOutOfRangeException ex)
        {
            return ApiResponse<KeyPointDto>.Fail("VALIDATION_ERROR", ex.Message);
        }
        catch (ArgumentException ex)
        {
            return ApiResponse<KeyPointDto>.Fail("VALIDATION_ERROR", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<KeyPointDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<TourDto>> PublishTourAsync(Guid guideId, Guid tourId)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<TourDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        if (tour.GuideId != guideId)
        {
            return ApiResponse<TourDto>.Fail("UNAUTHORIZED", "You are not the owner of this tour.");
        }

        try
        {
            tour.Publish();
            await _tourRepository.SaveChangesAsync();

            return ApiResponse<TourDto>.Ok(MapToDto(tour));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<TourDto>.Fail("PUBLISH_FAILED", ex.Message);
        }
    }

    public async Task<ApiResponse<List<TourDto>>> GetMyToursAsync(Guid guideId, bool sortAscending = true)
    {
        var tours = await _tourRepository.GetByGuideAsync(guideId, sortAscending);
        var dtos = tours.Select(MapToDto).ToList();
        return ApiResponse<List<TourDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<List<TourDto>>> GetPublishedToursAsync(bool sortAscending = true)
    {
        var tours = await _tourRepository.GetPublishedAsync(sortAscending);
        var dtos = tours.Select(MapToDto).ToList();
        return ApiResponse<List<TourDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<TourDto>> GetTourByIdAsync(Guid tourId, Guid? userId = null)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<TourDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        if (tour.Status != TourStatus.Published && tour.GuideId != userId)
        {
            return ApiResponse<TourDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        return ApiResponse<TourDto>.Ok(MapToDto(tour));
    }

    private static TourDto MapToDto(Tour tour)
    {
        return new TourDto
        {
            Id = tour.Id,
            GuideId = tour.GuideId,
            Name = tour.Name,
            Description = tour.Description,
            Difficulty = tour.Difficulty,
            Category = tour.Category,
            Price = tour.Price,
            StartDate = tour.StartDate,
            Status = tour.Status,
            KeyPoints = tour.KeyPoints.Select(MapKeyPointToDto).ToList()
        };
    }

    private static KeyPointDto MapKeyPointToDto(KeyPoint keyPoint)
    {
        return new KeyPointDto
        {
            Id = keyPoint.Id,
            Latitude = keyPoint.Latitude,
            Longitude = keyPoint.Longitude,
            Name = keyPoint.Name,
            Description = keyPoint.Description,
            ImageUrl = keyPoint.ImageUrl,
            Order = keyPoint.Order
        };
    }
}
