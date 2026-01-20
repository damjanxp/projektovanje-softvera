using FluentAssertions;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Application.Ratings.DTOs;
using TourApp.Application.Ratings.Interfaces;
using TourApp.Application.Ratings.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Purchases;
using TourApp.Domain.Ratings;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tests;

public class RatingServiceTests
{
    private readonly FakeRatingRepository _ratingRepository;
    private readonly FakeRatingPurchaseRepository _purchaseRepository;
    private readonly FakeRatingTourRepository _tourRepository;
    private readonly RatingService _ratingService;

    private readonly Guid _touristId = Guid.NewGuid();
    private readonly Guid _guideId = Guid.NewGuid();

    public RatingServiceTests()
    {
        _ratingRepository = new FakeRatingRepository();
        _purchaseRepository = new FakeRatingPurchaseRepository();
        _tourRepository = new FakeRatingTourRepository();

        _ratingService = new RatingService(
            _ratingRepository,
            _purchaseRepository,
            _tourRepository);
    }

    #region Not Purchased Tour Tests

    [Fact]
    public async Task CreateRating_WhenTouristHasNotPurchasedTour_ReturnsNotPurchasedError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(-3));
        _tourRepository.AddTour(tour);

        // Tourist has no purchases
        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 5,
            Comment = "Great tour!"
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("NOT_PURCHASED");
        result.Error.Message.Should().Be("You can only rate tours you have purchased.");
    }

    [Fact]
    public async Task CreateRating_WhenTouristPurchasedDifferentTour_ReturnsNotPurchasedError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var differentTourId = Guid.NewGuid();

        var tour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(-3));
        _tourRepository.AddTour(tour);

        // Tourist purchased a different tour
        var purchase = CreatePurchase(_touristId, differentTourId, "Different Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 5,
            Comment = "Great tour!"
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("NOT_PURCHASED");
    }

    [Fact]
    public async Task CreateRating_WhenTouristHasPurchasedTour_Succeeds()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(-3));
        _tourRepository.AddTour(tour);

        var purchase = CreatePurchase(_touristId, tourId, "Test Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 5,
            Comment = "Great tour!"
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Score.Should().Be(5);
        result.Data.TourId.Should().Be(tourId);
    }

    #endregion

    #region Tour Date Validation Tests

    [Fact]
    public async Task CreateRating_WhenTourHasNotTakenPlace_ReturnsInvalidOperationError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var futureTour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(5)); // Future tour
        _tourRepository.AddTour(futureTour);

        var purchase = CreatePurchase(_touristId, tourId, "Future Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 5,
            Comment = null
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("INVALID_OPERATION");
        result.Error.Message.Should().Contain("before it has taken place");
    }

    [Fact]
    public async Task CreateRating_WhenMoreThan7DaysSinceTour_ReturnsInvalidOperationError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var oldTour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(-10)); // 10 days ago
        _tourRepository.AddTour(oldTour);

        var purchase = CreatePurchase(_touristId, tourId, "Old Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 5,
            Comment = null
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("INVALID_OPERATION");
        result.Error.Message.Should().Contain("more than 7 days");
    }

    [Fact]
    public async Task CreateRating_Within7DaysOfTour_Succeeds()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var recentTour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(-5)); // 5 days ago
        _tourRepository.AddTour(recentTour);

        var purchase = CreatePurchase(_touristId, tourId, "Recent Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 4,
            Comment = "Good experience"
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Score.Should().Be(4);
    }

    #endregion

    #region Already Rated Tests

    [Fact]
    public async Task CreateRating_WhenAlreadyRated_ReturnsAlreadyRatedError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId, DateTime.UtcNow.AddDays(-3));
        _tourRepository.AddTour(tour);

        var purchase = CreatePurchase(_touristId, tourId, "Test Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        // Add existing rating
        var existingRating = new Rating(
            Guid.NewGuid(),
            _touristId,
            tourId,
            5,
            "Already rated",
            tour.StartDate);
        _ratingRepository.AddRating(existingRating);
        _ratingRepository.SetExistingRating(_touristId, tourId, existingRating);

        var request = new CreateRatingRequest
        {
            TourId = tourId,
            Score = 4,
            Comment = "Trying to rate again"
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("ALREADY_RATED");
        result.Error.Message.Should().Be("You have already rated this tour.");
    }

    #endregion

    #region Tour Not Found Tests

    [Fact]
    public async Task CreateRating_WhenTourNotFound_ReturnsTourNotFoundError()
    {
        // Arrange
        var nonExistentTourId = Guid.NewGuid();

        var request = new CreateRatingRequest
        {
            TourId = nonExistentTourId,
            Score = 5,
            Comment = "Great!"
        };

        // Act
        var result = await _ratingService.CreateRatingAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("TOUR_NOT_FOUND");
    }

    #endregion

    #region Helper Methods

    private Tour CreatePublishedTour(Guid tourId, DateTime startDate)
    {
        var tour = new Tour(
            tourId,
            _guideId,
            "Test Tour",
            "Test Description",
            Difficulty.Easy,
            Interest.Nature,
            100.00m,
            startDate);

        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();

        return tour;
    }

    private Purchase CreatePurchase(Guid touristId, Guid tourId, string tourName, decimal price)
    {
        var purchase = new Purchase(
            Guid.NewGuid(),
            touristId,
            price,
            0,
            10);

        purchase.AddPurchasedTour(tourId, tourName, price);
        return purchase;
    }

    #endregion
}

#region Fake Repositories for Rating Tests

public class FakeRatingRepository : IRatingRepository
{
    private readonly List<Rating> _ratings = new();
    private readonly Dictionary<(Guid touristId, Guid tourId), Rating> _existingRatings = new();

    public void AddRating(Rating rating)
    {
        _ratings.Add(rating);
    }

    public void SetExistingRating(Guid touristId, Guid tourId, Rating rating)
    {
        _existingRatings[(touristId, tourId)] = rating;
    }

    public Task AddAsync(Rating rating)
    {
        _ratings.Add(rating);
        return Task.CompletedTask;
    }

    public Task<Rating?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_ratings.FirstOrDefault(r => r.Id == id));
    }

    public Task<Rating?> GetByTouristAndTourAsync(Guid touristId, Guid tourId)
    {
        _existingRatings.TryGetValue((touristId, tourId), out var rating);
        return Task.FromResult(rating);
    }

    public Task<IEnumerable<Rating>> GetByTourAsync(Guid tourId)
    {
        return Task.FromResult(_ratings.Where(r => r.TourId == tourId));
    }

    public Task<double?> GetAverageRatingForTourAsync(Guid tourId)
    {
        var ratings = _ratings.Where(r => r.TourId == tourId).ToList();
        if (!ratings.Any()) return Task.FromResult<double?>(null);
        return Task.FromResult<double?>(ratings.Average(r => r.Score));
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}

public class FakeRatingPurchaseRepository : IPurchaseRepository
{
    private readonly List<Purchase> _purchases = new();

    public void AddPurchase(Purchase purchase)
    {
        _purchases.Add(purchase);
    }

    public Task AddAsync(Purchase purchase)
    {
        _purchases.Add(purchase);
        return Task.CompletedTask;
    }

    public Task<Purchase?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_purchases.FirstOrDefault(p => p.Id == id));
    }

    public Task<IEnumerable<Purchase>> GetByTouristAsync(Guid touristId)
    {
        return Task.FromResult(_purchases.Where(p => p.TouristId == touristId));
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}

public class FakeRatingTourRepository : ITourRepository
{
    private readonly List<Tour> _tours = new();

    public void AddTour(Tour tour)
    {
        _tours.Add(tour);
    }

    public Task AddAsync(Tour tour)
    {
        _tours.Add(tour);
        return Task.CompletedTask;
    }

    public Task<Tour?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_tours.FirstOrDefault(t => t.Id == id));
    }

    public Task<IEnumerable<Tour>> GetByGuideAsync(Guid guideId, bool sortAscending = true)
    {
        return Task.FromResult(_tours.Where(t => t.GuideId == guideId));
    }

    public Task<IEnumerable<Tour>> GetPublishedAsync(bool sortAscending = true)
    {
        return Task.FromResult(_tours.Where(t => t.Status == TourStatus.Published));
    }

    public Task<KeyPoint> AddKeyPointAsync(Tour tour, double latitude, double longitude, string name, string description, string imageUrl)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Tour>> GetToursNeedingReplacementAsync(Guid excludeGuideId)
    {
        return Task.FromResult(Enumerable.Empty<Tour>());
    }

    public Task<IEnumerable<Tour>> GetToursStartingWithin24HoursNeedingReplacementAsync()
    {
        return Task.FromResult(Enumerable.Empty<Tour>());
    }

    public Task<IEnumerable<Tour>> GetToursStartingIn48HoursAsync()
    {
        return Task.FromResult(Enumerable.Empty<Tour>());
    }

    public Task<bool> HasTourOnDateAsync(Guid guideId, DateTime date)
    {
        return Task.FromResult(false);
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}

#endregion
