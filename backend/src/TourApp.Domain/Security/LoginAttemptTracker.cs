namespace TourApp.Domain.Security;

public class LoginAttemptTracker
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public int FailedCount { get; private set; }
    public bool IsBlocked { get; private set; }
    public int BlockCount { get; private set; }
    public DateTime? LastFailedAt { get; private set; }
    public DateTime? BlockedAt { get; private set; }

    public LoginAttemptTracker(Guid id, string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        Id = id;
        Username = username;
        FailedCount = 0;
        IsBlocked = false;
        BlockCount = 0;
        LastFailedAt = null;
        BlockedAt = null;
    }

    private LoginAttemptTracker() { }

    public void RecordFailedAttempt()
    {
        FailedCount++;
        LastFailedAt = DateTime.UtcNow;

        if (FailedCount >= 5)
        {
            IsBlocked = true;
            BlockCount++;
            BlockedAt = DateTime.UtcNow;
        }
    }

    public void ResetFailedAttempts()
    {
        FailedCount = 0;
        LastFailedAt = null;
    }

    public bool CanBeUnblocked()
    {
        return IsBlocked && BlockCount < 3;
    }

    public void Unblock()
    {
        if (!CanBeUnblocked())
            throw new InvalidOperationException("User cannot be unblocked. Maximum block count reached.");

        IsBlocked = false;
        FailedCount = 0;
        BlockedAt = null;
    }
}
