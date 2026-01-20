using Microsoft.EntityFrameworkCore;
using TourApp.Application.Cancellations.DTOs;
using TourApp.Application.Purchases.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Purchases;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Application.Cancellations.Services;

public class CancellationService : ICancellationService
{
    private readonly ITourRepository _tourRepository;
    private readonly DbContext _dbContext;
    private readonly IEmailService _emailService;

    public CancellationService(
        ITourRepository tourRepository,
        DbContext dbContext,
        IEmailService emailService)
    {
        _tourRepository = tourRepository;
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task<ApiResponse<CancellationResultDto>> CancelToursWithoutReplacementAsync()
    {
        var toursToCancel = await _tourRepository.GetToursStartingWithin24HoursNeedingReplacementAsync();

        var result = new CancellationResultDto();

        foreach (var tour in toursToCancel)
        {
            var purchasedTours = await _dbContext.Set<PurchasedTour>()
                .Where(pt => pt.TourId == tour.Id)
                .ToListAsync();

            var touristIds = await _dbContext.Set<Purchase>()
                .Where(p => p.PurchasedTours.Any(pt => pt.TourId == tour.Id))
                .Select(p => p.TouristId)
                .Distinct()
                .ToListAsync();

            var bonusPointsToAward = (int)tour.Price;
            var canceledTourDto = new CanceledTourDto
            {
                TourId = tour.Id,
                TourName = tour.Name,
                StartDate = tour.StartDate,
                TouristsAffected = touristIds.Count,
                BonusPointsAwarded = bonusPointsToAward * touristIds.Count
            };

            foreach (var touristId in touristIds)
            {
                var tourist = await _dbContext.Set<Tourist>().FirstOrDefaultAsync(t => t.Id == touristId);
                if (tourist != null)
                {
                    tourist.AddBonusPoints(bonusPointsToAward);

                    await _emailService.SendTourCancellationAsync(
                        tourist.Email,
                        $"{tourist.FirstName} {tourist.LastName}",
                        tour.Name,
                        tour.StartDate,
                        bonusPointsToAward);

                    result.TouristsRefunded++;
                    result.TotalBonusPointsAwarded += bonusPointsToAward;
                }
            }

            tour.Cancel();
            result.ToursCanceled++;
            result.CanceledTours.Add(canceledTourDto);
        }

        await _tourRepository.SaveChangesAsync();

        return ApiResponse<CancellationResultDto>.Ok(result);
    }
}
