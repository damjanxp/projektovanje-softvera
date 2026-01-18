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
}
