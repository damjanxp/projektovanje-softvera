using TourApp.Application.Purchases.DTOs;

namespace TourApp.Application.Purchases.Services;

public interface IEmailService
{
    Task SendPurchaseConfirmationAsync(string toEmail, string touristName, PurchaseResultDto purchase);
}
