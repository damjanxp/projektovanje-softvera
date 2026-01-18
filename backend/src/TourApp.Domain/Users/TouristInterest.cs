namespace TourApp.Domain.Users;

public class TouristInterest
{
    public Guid TouristId { get; private set; }
    public Interest Interest { get; private set; }

    public TouristInterest(Guid touristId, Interest interest)
    {
        TouristId = touristId;
        Interest = interest;
    }

    private TouristInterest() { }
}
