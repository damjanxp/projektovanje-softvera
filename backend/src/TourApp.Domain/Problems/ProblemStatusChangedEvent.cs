namespace TourApp.Domain.Problems;

public class ProblemStatusChangedEvent
{
    public Guid Id { get; private set; }
    public Guid ProblemId { get; private set; }
    public ProblemStatus OldStatus { get; private set; }
    public ProblemStatus NewStatus { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public string ChangedByRole { get; private set; }
    public Guid ChangedByUserId { get; private set; }

    public ProblemStatusChangedEvent(
        Guid id,
        Guid problemId,
        ProblemStatus oldStatus,
        ProblemStatus newStatus,
        string changedByRole,
        Guid changedByUserId)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Event Id cannot be empty.", nameof(id));

        if (problemId == Guid.Empty)
            throw new ArgumentException("ProblemId cannot be empty.", nameof(problemId));

        if (string.IsNullOrWhiteSpace(changedByRole))
            throw new ArgumentException("ChangedByRole cannot be null or empty.", nameof(changedByRole));

        if (changedByUserId == Guid.Empty)
            throw new ArgumentException("ChangedByUserId cannot be empty.", nameof(changedByUserId));

        Id = id;
        ProblemId = problemId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = DateTime.UtcNow;
        ChangedByRole = changedByRole;
        ChangedByUserId = changedByUserId;
    }

    private ProblemStatusChangedEvent() { }
}
