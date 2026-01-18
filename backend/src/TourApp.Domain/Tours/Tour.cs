using TourApp.Domain.Users;

namespace TourApp.Domain.Tours;

public class Tour
{
    private readonly List<KeyPoint> _keyPoints = new();

    public Guid Id { get; private set; }
    public Guid GuideId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Difficulty Difficulty { get; private set; }
    public Interest Category { get; private set; }
    public decimal Price { get; private set; }
    public DateTime StartDate { get; private set; }
    public TourStatus Status { get; private set; }
    public IReadOnlyCollection<KeyPoint> KeyPoints => _keyPoints.AsReadOnly();

    public Tour(
        Guid id,
        Guid guideId,
        string name,
        string description,
        Difficulty difficulty,
        Interest category,
        decimal price,
        DateTime startDate)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Tour Id cannot be empty.", nameof(id));

        if (guideId == Guid.Empty)
            throw new ArgumentException("GuideId cannot be empty.", nameof(guideId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be null or empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");

        Id = id;
        GuideId = guideId;
        Name = name;
        Description = description;
        Difficulty = difficulty;
        Category = category;
        Price = price;
        StartDate = startDate;
        Status = TourStatus.Draft;
    }

    private Tour() { }

    public void AddKeyPoint(double latitude, double longitude, string name, string description, string imageUrl)
    {
        if (Status == TourStatus.Published)
            throw new InvalidOperationException("Cannot add key points to a published tour.");

        if (Status == TourStatus.Canceled)
            throw new InvalidOperationException("Cannot add key points to a canceled tour.");

        var order = _keyPoints.Count;
        var keyPoint = new KeyPoint(Guid.NewGuid(), Id, latitude, longitude, name, description, imageUrl, order);
        _keyPoints.Add(keyPoint);
    }

    public void Publish()
    {
        if (Status == TourStatus.Published)
            throw new InvalidOperationException("Tour is already published.");

        if (Status == TourStatus.Canceled)
            throw new InvalidOperationException("Cannot publish a canceled tour.");

        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidOperationException("Tour must have a name before publishing.");

        if (string.IsNullOrWhiteSpace(Description))
            throw new InvalidOperationException("Tour must have a description before publishing.");

        if (_keyPoints.Count < 2)
            throw new InvalidOperationException("Tour must have at least 2 key points before publishing.");

        Status = TourStatus.Published;
    }

    public void Cancel()
    {
        if (Status == TourStatus.Canceled)
            throw new InvalidOperationException("Tour is already canceled.");

        Status = TourStatus.Canceled;
    }
}
