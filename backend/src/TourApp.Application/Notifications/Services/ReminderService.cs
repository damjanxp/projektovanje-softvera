using Microsoft.EntityFrameworkCore;
using TourApp.Application.Notifications.DTOs;
using TourApp.Application.Purchases.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Purchases;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Application.Notifications.Services;

public class ReminderService : IReminderService
{
    private readonly ITourRepository _tourRepository;
    private readonly DbContext _dbContext;
    private readonly IEmailService _emailService;

    public ReminderService(
        ITourRepository tourRepository,
        DbContext dbContext,
        IEmailService emailService)
    {
        _tourRepository = tourRepository;
        _dbContext = dbContext;
        _emailService = emailService;
    }

    public async Task<ApiResponse<ReminderResultDto>> SendTourRemindersAsync()
    {
        var toursIn48Hours = await _tourRepository.GetToursStartingIn48HoursAsync();

        var result = new ReminderResultDto
        {
            TourCount = toursIn48Hours.Count()
        };

        foreach (var tour in toursIn48Hours)
        {
            var touristIds = await _dbContext.Set<Purchase>()
                .Where(p => p.PurchasedTours.Any(pt => pt.TourId == tour.Id))
                .Select(p => p.TouristId)
                .Distinct()
                .ToListAsync();

            var tourReminderDto = new TourReminderDto
            {
                TourId = tour.Id,
                TourName = tour.Name,
                StartDate = tour.StartDate,
                TouristsNotified = 0
            };

            foreach (var touristId in touristIds)
            {
                var tourist = await _dbContext.Set<Tourist>().FirstOrDefaultAsync(t => t.Id == touristId);
                if (tourist != null)
                {
                    await _emailService.SendTourReminderAsync(
                        tourist.Email,
                        $"{tourist.FirstName} {tourist.LastName}",
                        tour.Name,
                        tour.StartDate,
                        tour.Description);

                    tourReminderDto.TouristsNotified++;
                    result.RemindersSent++;
                }
            }

            result.Tours.Add(tourReminderDto);
        }

        return ApiResponse<ReminderResultDto>.Ok(result);
    }
}
