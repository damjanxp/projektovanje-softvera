using Microsoft.Extensions.Logging;
using TourApp.Application.Purchases.DTOs;
using TourApp.Application.Purchases.Services;

namespace TourApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public Task SendPurchaseConfirmationAsync(string toEmail, string touristName, PurchaseResultDto purchase)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("?? PURCHASE CONFIRMATION EMAIL");
        _logger.LogInformation("========================================");
        _logger.LogInformation("To: {Email}", toEmail);
        _logger.LogInformation("Dear {Name},", touristName);
        _logger.LogInformation("");
        _logger.LogInformation("Thank you for your purchase!");
        _logger.LogInformation("Purchase ID: {PurchaseId}", purchase.PurchaseId);
        _logger.LogInformation("Date: {Date}", purchase.PurchasedAt);
        _logger.LogInformation("");
        _logger.LogInformation("Tours purchased:");

        foreach (var tour in purchase.Tours)
        {
            _logger.LogInformation("  - {TourName}: ${Price}", tour.TourName, tour.PriceAtPurchase);
        }

        _logger.LogInformation("");
        _logger.LogInformation("Original Price: ${OriginalPrice}", purchase.OriginalPrice);
        
        if (purchase.BonusPointsUsed > 0)
        {
            _logger.LogInformation("Bonus Points Used: {Points} (-${Discount})", purchase.BonusPointsUsed, purchase.BonusPointsUsed);
        }
        
        _logger.LogInformation("Final Price: ${FinalPrice}", purchase.FinalPrice);
        _logger.LogInformation("");
        _logger.LogInformation("You earned {Points} bonus points with this purchase!", purchase.BonusPointsEarned);
        _logger.LogInformation("Your total bonus points: {TotalPoints}", purchase.TotalBonusPoints);
        _logger.LogInformation("");
        _logger.LogInformation("Enjoy your tours!");
        _logger.LogInformation("TourApp Team");
        _logger.LogInformation("========================================");

        return Task.CompletedTask;
    }

    public Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime startDate, string description)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("? TOUR REMINDER EMAIL");
        _logger.LogInformation("========================================");
        _logger.LogInformation("To: {Email}", toEmail);
        _logger.LogInformation("Dear {Name},", touristName);
        _logger.LogInformation("");
        _logger.LogInformation("This is a reminder that your tour is starting soon!");
        _logger.LogInformation("");
        _logger.LogInformation("Tour: {TourName}", tourName);
        _logger.LogInformation("Date: {StartDate:dddd, MMMM dd, yyyy 'at' HH:mm}", startDate);
        _logger.LogInformation("");
        _logger.LogInformation("Description: {Description}", description);
        _logger.LogInformation("");
        _logger.LogInformation("Please arrive at the meeting point 15 minutes before the start time.");
        _logger.LogInformation("");
        _logger.LogInformation("We look forward to seeing you!");
        _logger.LogInformation("TourApp Team");
        _logger.LogInformation("========================================");

        return Task.CompletedTask;
    }

    public Task SendTourCancellationAsync(string toEmail, string touristName, string tourName, DateTime startDate, int bonusPointsAwarded)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("? TOUR CANCELLATION EMAIL");
        _logger.LogInformation("========================================");
        _logger.LogInformation("To: {Email}", toEmail);
        _logger.LogInformation("Dear {Name},", touristName);
        _logger.LogInformation("");
        _logger.LogInformation("We regret to inform you that the following tour has been canceled:");
        _logger.LogInformation("");
        _logger.LogInformation("Tour: {TourName}", tourName);
        _logger.LogInformation("Original Date: {StartDate:dddd, MMMM dd, yyyy 'at' HH:mm}", startDate);
        _logger.LogInformation("");
        _logger.LogInformation("As compensation, you have been awarded {BonusPoints} bonus points.", bonusPointsAwarded);
        _logger.LogInformation("You can use these points for future purchases.");
        _logger.LogInformation("");
        _logger.LogInformation("We apologize for any inconvenience caused.");
        _logger.LogInformation("TourApp Team");
        _logger.LogInformation("========================================");

        return Task.CompletedTask;
    }

    public Task SendProblemNotificationToGuideAsync(Guid guideId, string tourName, string problemTitle)
    {
        _logger.LogInformation("========================================");
        _logger.LogInformation("?? NEW PROBLEM REPORTED EMAIL");
        _logger.LogInformation("========================================");
        _logger.LogInformation("To Guide: {GuideId}", guideId);
        _logger.LogInformation("");
        _logger.LogInformation("A new problem has been reported for your tour:");
        _logger.LogInformation("");
        _logger.LogInformation("Tour: {TourName}", tourName);
        _logger.LogInformation("Problem: {ProblemTitle}", problemTitle);
        _logger.LogInformation("");
        _logger.LogInformation("Please review this problem in your dashboard.");
        _logger.LogInformation("TourApp Team");
        _logger.LogInformation("========================================");

        return Task.CompletedTask;
    }
}
