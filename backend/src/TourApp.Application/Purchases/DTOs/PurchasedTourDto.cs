namespace TourApp.Application.Purchases.DTOs;

public class PurchasedTourDto
{
    public Guid TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public decimal PriceAtPurchase { get; set; }
}
