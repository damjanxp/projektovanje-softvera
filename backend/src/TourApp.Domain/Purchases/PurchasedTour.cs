namespace TourApp.Domain.Purchases;

public class PurchasedTour
{
    public Guid Id { get; private set; }
    public Guid PurchaseId { get; private set; }
    public Guid TourId { get; private set; }
    public string TourName { get; private set; }
    public decimal PriceAtPurchase { get; private set; }

    public PurchasedTour(Guid id, Guid purchaseId, Guid tourId, string tourName, decimal priceAtPurchase)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("PurchasedTour Id cannot be empty.", nameof(id));

        if (purchaseId == Guid.Empty)
            throw new ArgumentException("PurchaseId cannot be empty.", nameof(purchaseId));

        if (tourId == Guid.Empty)
            throw new ArgumentException("TourId cannot be empty.", nameof(tourId));

        if (string.IsNullOrWhiteSpace(tourName))
            throw new ArgumentException("TourName cannot be null or empty.", nameof(tourName));

        if (priceAtPurchase < 0)
            throw new ArgumentOutOfRangeException(nameof(priceAtPurchase), "Price cannot be negative.");

        Id = id;
        PurchaseId = purchaseId;
        TourId = tourId;
        TourName = tourName;
        PriceAtPurchase = priceAtPurchase;
    }

    private PurchasedTour() { }
}
