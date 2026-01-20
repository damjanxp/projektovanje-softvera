namespace TourApp.Domain.Ratings;

public class Rating
{
    public Guid Id { get; private set; }
    public Guid TouristId { get; private set; }
    public Guid TourId { get; private set; }
    public int Score { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Rating(
        Guid id,
        Guid touristId,
        Guid tourId,
        int score,
        string? comment,
        DateTime tourStartDate)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Rating Id cannot be empty.", nameof(id));

        if (touristId == Guid.Empty)
            throw new ArgumentException("TouristId cannot be empty.", nameof(touristId));

        if (tourId == Guid.Empty)
            throw new ArgumentException("TourId cannot be empty.", nameof(tourId));

        if (score < 1 || score > 5)
            throw new ArgumentOutOfRangeException(nameof(score), "Score must be between 1 and 5.");

        // Validate that rating is after tour date
        if (DateTime.UtcNow < tourStartDate)
            throw new InvalidOperationException("Cannot rate a tour before it has taken place.");

        // Validate that rating is within 7 days of tour date
        var daysSinceTour = (DateTime.UtcNow - tourStartDate).TotalDays;
        if (daysSinceTour >= 7.01) // Allow some tolerance for timing precision
            throw new InvalidOperationException("Cannot rate a tour more than 7 days after it has taken place.");

        // Validate that low scores require a comment
        if (score <= 2 && string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("A comment is required for ratings of 1 or 2.", nameof(comment));

        Id = id;
        TouristId = touristId;
        TourId = tourId;
        Score = score;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }

    private Rating() { }
}
