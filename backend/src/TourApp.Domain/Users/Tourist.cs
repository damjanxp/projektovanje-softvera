namespace TourApp.Domain.Users;

public class Tourist
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string PasswordHash { get; private set; }
    public bool WantsRecommendations { get; private set; }
    public int BonusPoints { get; private set; }

    private readonly List<TouristInterest> _interests = new();
    public IReadOnlyCollection<TouristInterest> Interests => _interests.AsReadOnly();

    public Tourist(
        Guid id, 
        string username, 
        string email, 
        string firstName, 
        string lastName,
        string passwordHash,
        bool wantsRecommendations,
        IEnumerable<Interest> interests)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

        Id = id;
        Username = username;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PasswordHash = passwordHash;
        WantsRecommendations = wantsRecommendations;
        BonusPoints = 0;

        foreach (var interest in interests)
        {
            _interests.Add(new TouristInterest(id, interest));
        }
    }

    private Tourist() { }

    public void AddBonusPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Cannot add negative bonus points.", nameof(points));

        BonusPoints += points;
    }

    public void SpendBonusPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Cannot spend negative bonus points.", nameof(points));

        if (points > BonusPoints)
            throw new InvalidOperationException($"Cannot spend {points} bonus points. Available: {BonusPoints}.");

        BonusPoints -= points;
    }
}
