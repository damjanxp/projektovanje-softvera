namespace TourApp.Domain.Tours;

public class KeyPoint
{
    public Guid Id { get; private set; }
    public Guid TourId { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string ImageUrl { get; private set; }
    public int Order { get; private set; }

    public KeyPoint(Guid id, Guid tourId, double latitude, double longitude, string name, string description, string imageUrl, int order)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("KeyPoint Id cannot be empty.", nameof(id));

        if (tourId == Guid.Empty)
            throw new ArgumentException("TourId cannot be empty.", nameof(tourId));

        if (latitude < -90 || latitude > 90)
            throw new ArgumentOutOfRangeException(nameof(latitude), "Latitude must be between -90 and 90.");

        if (longitude < -180 || longitude > 180)
            throw new ArgumentOutOfRangeException(nameof(longitude), "Longitude must be between -180 and 180.");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));

        if (order < 0)
            throw new ArgumentOutOfRangeException(nameof(order), "Order must be non-negative.");

        Id = id;
        TourId = tourId;
        Latitude = latitude;
        Longitude = longitude;
        Name = name;
        Description = description;
        ImageUrl = imageUrl ?? string.Empty;
        Order = order;
    }

    private KeyPoint() { }
}
