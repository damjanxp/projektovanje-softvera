using TourApp.Application.Purchases.DTOs;

namespace TourApp.Application.Purchases.Services;

public interface IEmailService
{
    Task SendPurchaseConfirmationAsync(string toEmail, string touristName, PurchaseResultDto purchase);
    Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime startDate, string description);
    Task SendTourCancellationAsync(string toEmail, string touristName, string tourName, DateTime startDate, int bonusPointsAwarded);
    Task SendProblemNotificationToGuideAsync(Guid guideId, string tourName, string problemTitle);
}
