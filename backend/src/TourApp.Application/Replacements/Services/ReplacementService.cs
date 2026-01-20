using TourApp.Application.Replacements.DTOs;
using TourApp.Application.Tours.DTOs;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Tours;
using TourApp.Shared;

namespace TourApp.Application.Replacements.Services;

public class ReplacementService : IReplacementService
{
    private readonly ITourRepository _tourRepository;

    public ReplacementService(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public async Task<ApiResponse<TourDto>> RequestReplacementAsync(Guid guideId, Guid tourId)
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
            tour.RequestReplacement();
            await _tourRepository.SaveChangesAsync();

            return ApiResponse<TourDto>.Ok(MapToTourDto(tour));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<TourDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<TourDto>> CancelReplacementRequestAsync(Guid guideId, Guid tourId)
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
            tour.CancelReplacement();
            await _tourRepository.SaveChangesAsync();

            return ApiResponse<TourDto>.Ok(MapToTourDto(tour));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<TourDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    public async Task<ApiResponse<List<ReplacementTourDto>>> GetReplacementToursAsync(Guid guideId)
    {
        var tours = await _tourRepository.GetToursNeedingReplacementAsync(guideId);

        var dtos = tours.Select(t => new ReplacementTourDto
        {
            Id = t.Id,
            CurrentGuideId = t.GuideId,
            Name = t.Name,
            Description = t.Description,
            Difficulty = t.Difficulty,
            Category = t.Category,
            Price = t.Price,
            StartDate = t.StartDate,
            ReplacementRequestedAt = t.ReplacementRequestedAt
        }).ToList();

        return ApiResponse<List<ReplacementTourDto>>.Ok(dtos);
    }

    public async Task<ApiResponse<TourDto>> TakeOverTourAsync(Guid newGuideId, Guid tourId)
    {
        var tour = await _tourRepository.GetByIdAsync(tourId);
        if (tour == null)
        {
            return ApiResponse<TourDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        if (!tour.NeedsReplacement)
        {
            return ApiResponse<TourDto>.Fail("NOT_AVAILABLE", "This tour is not available for takeover.");
        }

        if (tour.GuideId == newGuideId)
        {
            return ApiResponse<TourDto>.Fail("INVALID_OPERATION", "You cannot take over your own tour.");
        }

        var hasConflict = await _tourRepository.HasTourOnDateAsync(newGuideId, tour.StartDate);
        if (hasConflict)
        {
            return ApiResponse<TourDto>.Fail("SCHEDULE_CONFLICT", "You already have a tour scheduled on this date.");
        }

        try
        {
            tour.AssignNewGuide(newGuideId);
            await _tourRepository.SaveChangesAsync();

            return ApiResponse<TourDto>.Ok(MapToTourDto(tour));
        }
        catch (InvalidOperationException ex)
        {
            return ApiResponse<TourDto>.Fail("INVALID_OPERATION", ex.Message);
        }
    }

    private static TourDto MapToTourDto(Tour tour)
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
            KeyPoints = tour.KeyPoints.Select(kp => new KeyPointDto
            {
                Id = kp.Id,
                Latitude = kp.Latitude,
                Longitude = kp.Longitude,
                Name = kp.Name,
                Description = kp.Description,
                ImageUrl = kp.ImageUrl,
                Order = kp.Order
            }).ToList()
        };
    }
}
