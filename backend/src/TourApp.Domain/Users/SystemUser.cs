namespace TourApp.Domain.Users;

public class SystemUser
{
    public Guid Id { get; private set; }
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public Role Role { get; private set; }
    public string PasswordHash { get; private set; }

    public SystemUser(Guid id, string username, string email, string firstName, string lastName, Role role, string passwordHash)
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
        Role = role;
        PasswordHash = passwordHash;
    }

    private SystemUser() { }
}
