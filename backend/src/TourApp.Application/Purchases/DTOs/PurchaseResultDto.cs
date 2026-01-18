namespace TourApp.Application.Purchases.DTOs;

public class PurchaseResultDto
{
    public Guid PurchaseId { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public int BonusPointsUsed { get; set; }
    public int BonusPointsEarned { get; set; }
    public int TotalBonusPoints { get; set; }
    public DateTime PurchasedAt { get; set; }
    public List<PurchasedTourDto> Tours { get; set; } = new();
}
