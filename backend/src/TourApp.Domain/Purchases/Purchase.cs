namespace TourApp.Domain.Purchases;

public class Purchase
{
    private readonly List<PurchasedTour> _purchasedTours = new();

    public Guid Id { get; private set; }
    public Guid TouristId { get; private set; }
    public DateTime PurchasedAt { get; private set; }
    public decimal TotalPrice { get; private set; }
    public int BonusPointsUsed { get; private set; }
    public int BonusPointsEarned { get; private set; }
    public IReadOnlyCollection<PurchasedTour> PurchasedTours => _purchasedTours.AsReadOnly();

    public Purchase(
        Guid id,
        Guid touristId,
        decimal totalPrice,
        int bonusPointsUsed,
        int bonusPointsEarned)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Purchase Id cannot be empty.", nameof(id));

        if (touristId == Guid.Empty)
            throw new ArgumentException("TouristId cannot be empty.", nameof(touristId));

        if (totalPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(totalPrice), "Total price cannot be negative.");

        if (bonusPointsUsed < 0)
            throw new ArgumentOutOfRangeException(nameof(bonusPointsUsed), "Bonus points used cannot be negative.");

        if (bonusPointsEarned < 0)
            throw new ArgumentOutOfRangeException(nameof(bonusPointsEarned), "Bonus points earned cannot be negative.");

        Id = id;
        TouristId = touristId;
        PurchasedAt = DateTime.UtcNow;
        TotalPrice = totalPrice;
        BonusPointsUsed = bonusPointsUsed;
        BonusPointsEarned = bonusPointsEarned;
    }

    private Purchase() { }

    public void AddPurchasedTour(Guid tourId, string tourName, decimal priceAtPurchase)
    {
        var purchasedTour = new PurchasedTour(Guid.NewGuid(), Id, tourId, tourName, priceAtPurchase);
        _purchasedTours.Add(purchasedTour);
    }
}
