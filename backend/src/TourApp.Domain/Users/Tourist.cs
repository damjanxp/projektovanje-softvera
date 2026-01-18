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

        foreach (var interest in interests)
        {
            _interests.Add(new TouristInterest(id, interest));
        }
    }

    private Tourist() { }
}
