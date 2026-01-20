namespace TourApp.Domain.Problems;

public class Problem
{
    private readonly List<ProblemStatusChangedEvent> _events = new();

    public Guid Id { get; private set; }
    public Guid TouristId { get; private set; }
    public Guid TourId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public ProblemStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public IReadOnlyCollection<ProblemStatusChangedEvent> Events => _events.AsReadOnly();

    public Problem(
        Guid id,
        Guid touristId,
        Guid tourId,
        string title,
        string description)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Problem Id cannot be empty.", nameof(id));

        if (touristId == Guid.Empty)
            throw new ArgumentException("TouristId cannot be empty.", nameof(touristId));

        if (tourId == Guid.Empty)
            throw new ArgumentException("TourId cannot be empty.", nameof(tourId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be null or empty.", nameof(description));

        Id = id;
        TouristId = touristId;
        TourId = tourId;
        Title = title;
        Description = description;
        Status = ProblemStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    private Problem() { }

    public ProblemStatusChangedEvent Resolve(Guid guideId)
    {
        if (Status != ProblemStatus.Pending)
            throw new InvalidOperationException("Only pending problems can be resolved.");

        var oldStatus = Status;
        Status = ProblemStatus.Resolved;

        var statusEvent = new ProblemStatusChangedEvent(
            Guid.NewGuid(),
            Id,
            oldStatus,
            Status,
            "Guide",
            guideId);

        _events.Add(statusEvent);
        return statusEvent;
    }

    public ProblemStatusChangedEvent SendToReview(Guid guideId)
    {
        if (Status != ProblemStatus.Pending)
            throw new InvalidOperationException("Only pending problems can be sent for review.");

        var oldStatus = Status;
        Status = ProblemStatus.InReview;

        var statusEvent = new ProblemStatusChangedEvent(
            Guid.NewGuid(),
            Id,
            oldStatus,
            Status,
            "Guide",
            guideId);

        _events.Add(statusEvent);
        return statusEvent;
    }

    public ProblemStatusChangedEvent Reject(Guid adminId)
    {
        if (Status != ProblemStatus.InReview)
            throw new InvalidOperationException("Only problems in review can be rejected.");

        var oldStatus = Status;
        Status = ProblemStatus.Rejected;

        var statusEvent = new ProblemStatusChangedEvent(
            Guid.NewGuid(),
            Id,
            oldStatus,
            Status,
            "Admin",
            adminId);

        _events.Add(statusEvent);
        return statusEvent;
    }

    public ProblemStatusChangedEvent Reopen(Guid adminId)
    {
        if (Status != ProblemStatus.InReview)
            throw new InvalidOperationException("Only problems in review can be reopened.");

        var oldStatus = Status;
        Status = ProblemStatus.Pending;

        var statusEvent = new ProblemStatusChangedEvent(
            Guid.NewGuid(),
            Id,
            oldStatus,
            Status,
            "Admin",
            adminId);

        _events.Add(statusEvent);
        return statusEvent;
    }
}
