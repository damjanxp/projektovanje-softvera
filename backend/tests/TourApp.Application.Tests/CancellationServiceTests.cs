using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TourApp.Application.Cancellations.Services;
using TourApp.Application.Purchases.DTOs;
using TourApp.Application.Purchases.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Purchases;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tests;

public class CancellationServiceTests : IDisposable
{
    private readonly CancellationTestDbContext _dbContext;
    private readonly FakeTourRepository _tourRepository;
    private readonly CancellationFakeEmailService _emailService;
    private readonly CancellationService _cancellationService;

    public CancellationServiceTests()
    {
        var options = new DbContextOptionsBuilder<CancellationTestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new CancellationTestDbContext(options);
        _tourRepository = new FakeTourRepository();
        _emailService = new CancellationFakeEmailService();

        _cancellationService = new CancellationService(
            _tourRepository,
            _dbContext,
            _emailService);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public async Task CancelToursWithoutReplacement_AwardsBonusPointsToAllTouristsWhoPurchasedTour()
    {
        // Arrange
        var guideId = Guid.NewGuid();
        var tourId = Guid.NewGuid();
        var tourPrice = 150.00m;

        // Create a tour that needs replacement and starts within 24 hours
        var tour = new Tour(
            tourId,
            guideId,
            "Test Tour to Cancel",
            "Test Description",
            Difficulty.Medium,
            Interest.Nature,
            tourPrice,
            DateTime.UtcNow.AddHours(12)); // Starts in 12 hours

        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();
        tour.RequestReplacement();

        _tourRepository.AddTourNeedingReplacement(tour);

        // Create 3 tourists who purchased this tour
        var tourist1Id = Guid.NewGuid();
        var tourist2Id = Guid.NewGuid();
        var tourist3Id = Guid.NewGuid();

        var tourist1 = new Tourist(tourist1Id, "tourist1", "tourist1@example.com", "John", "Doe", "hash1", true, new[] { Interest.Nature });
        var tourist2 = new Tourist(tourist2Id, "tourist2", "tourist2@example.com", "Jane", "Smith", "hash2", true, new[] { Interest.Art });
        var tourist3 = new Tourist(tourist3Id, "tourist3", "tourist3@example.com", "Bob", "Wilson", "hash3", false, new[] { Interest.Food });

        _dbContext.Tourists.AddRange(tourist1, tourist2, tourist3);

        // Create purchases for each tourist
        var purchase1 = new Purchase(Guid.NewGuid(), tourist1Id, tourPrice, 0, 10);
        purchase1.AddPurchasedTour(tourId, "Test Tour to Cancel", tourPrice);

        var purchase2 = new Purchase(Guid.NewGuid(), tourist2Id, tourPrice, 0, 10);
        purchase2.AddPurchasedTour(tourId, "Test Tour to Cancel", tourPrice);

        var purchase3 = new Purchase(Guid.NewGuid(), tourist3Id, tourPrice, 0, 10);
        purchase3.AddPurchasedTour(tourId, "Test Tour to Cancel", tourPrice);

        _dbContext.Purchases.AddRange(purchase1, purchase2, purchase3);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cancellationService.CancelToursWithoutReplacementAsync();

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.ToursCanceled.Should().Be(1);
        result.Data.TouristsRefunded.Should().Be(3);

        // Each tourist should receive bonus points equal to tour price (150)
        var expectedBonusPoints = (int)tourPrice; // 150

        var updatedTourist1 = await _dbContext.Tourists.FirstAsync(t => t.Id == tourist1Id);
        var updatedTourist2 = await _dbContext.Tourists.FirstAsync(t => t.Id == tourist2Id);
        var updatedTourist3 = await _dbContext.Tourists.FirstAsync(t => t.Id == tourist3Id);

        updatedTourist1.BonusPoints.Should().Be(expectedBonusPoints);
        updatedTourist2.BonusPoints.Should().Be(expectedBonusPoints);
        updatedTourist3.BonusPoints.Should().Be(expectedBonusPoints);

        // Total bonus points awarded should be 150 * 3 = 450
        result.Data.TotalBonusPointsAwarded.Should().Be(expectedBonusPoints * 3);
    }

    [Fact]
    public async Task CancelToursWithoutReplacement_TourStatusBecomesCanceled()
    {
        // Arrange
        var guideId = Guid.NewGuid();
        var tourId = Guid.NewGuid();

        var tour = new Tour(
            tourId,
            guideId,
            "Tour to Cancel",
            "Description",
            Difficulty.Easy,
            Interest.Art,
            100.00m,
            DateTime.UtcNow.AddHours(10));

        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();
        tour.RequestReplacement();

        _tourRepository.AddTourNeedingReplacement(tour);

        // Act
        var result = await _cancellationService.CancelToursWithoutReplacementAsync();

        // Assert
        result.Success.Should().BeTrue();
        tour.Status.Should().Be(TourStatus.Canceled);
    }

    [Fact]
    public async Task CancelToursWithoutReplacement_BonusPointsEqualTourPrice()
    {
        // Arrange
        var guideId = Guid.NewGuid();
        var tourId = Guid.NewGuid();
        var tourPrice = 75.50m;

        var tour = new Tour(
            tourId,
            guideId,
            "Specific Price Tour",
            "Description",
            Difficulty.Hard,
            Interest.Sport,
            tourPrice,
            DateTime.UtcNow.AddHours(5));

        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();
        tour.RequestReplacement();

        _tourRepository.AddTourNeedingReplacement(tour);

        var touristId = Guid.NewGuid();
        var tourist = new Tourist(touristId, "tourist", "tourist@example.com", "John", "Doe", "hash", true, new[] { Interest.Sport });
        _dbContext.Tourists.Add(tourist);

        var purchase = new Purchase(Guid.NewGuid(), touristId, tourPrice, 0, 7);
        purchase.AddPurchasedTour(tourId, "Specific Price Tour", tourPrice);
        _dbContext.Purchases.Add(purchase);

        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cancellationService.CancelToursWithoutReplacementAsync();

        // Assert
        result.Success.Should().BeTrue();

        var updatedTourist = await _dbContext.Tourists.FirstAsync(t => t.Id == touristId);
        updatedTourist.BonusPoints.Should().Be((int)tourPrice); // 75 (truncated from 75.50)
    }

    [Fact]
    public async Task CancelToursWithoutReplacement_NoToursToCancel_ReturnsEmptyResult()
    {
        // Arrange - no tours added to repository

        // Act
        var result = await _cancellationService.CancelToursWithoutReplacementAsync();

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.ToursCanceled.Should().Be(0);
        result.Data.TouristsRefunded.Should().Be(0);
        result.Data.TotalBonusPointsAwarded.Should().Be(0);
    }

    [Fact]
    public async Task CancelToursWithoutReplacement_SendsEmailToAllAffectedTourists()
    {
        // Arrange
        var guideId = Guid.NewGuid();
        var tourId = Guid.NewGuid();
        var tourPrice = 100.00m;

        var tour = new Tour(
            tourId,
            guideId,
            "Email Test Tour",
            "Description",
            Difficulty.Easy,
            Interest.Nature,
            tourPrice,
            DateTime.UtcNow.AddHours(8));

        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();
        tour.RequestReplacement();

        _tourRepository.AddTourNeedingReplacement(tour);

        var tourist1Id = Guid.NewGuid();
        var tourist2Id = Guid.NewGuid();

        var tourist1 = new Tourist(tourist1Id, "tourist1", "tourist1@example.com", "John", "Doe", "hash1", true, new[] { Interest.Nature });
        var tourist2 = new Tourist(tourist2Id, "tourist2", "tourist2@example.com", "Jane", "Smith", "hash2", true, new[] { Interest.Nature });

        _dbContext.Tourists.AddRange(tourist1, tourist2);

        var purchase1 = new Purchase(Guid.NewGuid(), tourist1Id, tourPrice, 0, 10);
        purchase1.AddPurchasedTour(tourId, "Email Test Tour", tourPrice);

        var purchase2 = new Purchase(Guid.NewGuid(), tourist2Id, tourPrice, 0, 10);
        purchase2.AddPurchasedTour(tourId, "Email Test Tour", tourPrice);

        _dbContext.Purchases.AddRange(purchase1, purchase2);
        await _dbContext.SaveChangesAsync();

        // Act
        await _cancellationService.CancelToursWithoutReplacementAsync();

        // Assert
        _emailService.CancellationEmailsSent.Should().Be(2);
    }
}

#region Fake Implementations for Cancellation Tests

public class CancellationTestDbContext : DbContext
{
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<Tourist> Tourists { get; set; } = null!;
    public DbSet<Purchase> Purchases { get; set; } = null!;
    public DbSet<PurchasedTour> PurchasedTours { get; set; } = null!;

    public CancellationTestDbContext(DbContextOptions<CancellationTestDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasMany(t => t.KeyPoints)
                .WithOne()
                .HasForeignKey(kp => kp.TourId);
            entity.Navigation(t => t.KeyPoints).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<KeyPoint>(entity =>
        {
            entity.HasKey(k => k.Id);
        });

        modelBuilder.Entity<Tourist>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.HasMany(t => t.Interests)
                .WithOne()
                .HasForeignKey(ti => ti.TouristId);
            entity.Navigation(t => t.Interests).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<TouristInterest>(entity =>
        {
            entity.HasKey(ti => new { ti.TouristId, ti.Interest });
        });

        modelBuilder.Entity<Purchase>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.HasMany(p => p.PurchasedTours)
                .WithOne()
                .HasForeignKey(pt => pt.PurchaseId);
            entity.Navigation(p => p.PurchasedTours).UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<PurchasedTour>(entity =>
        {
            entity.HasKey(pt => pt.Id);
        });
    }
}

public class FakeTourRepository : ITourRepository
{
    private readonly List<Tour> _toursNeedingReplacement = new();
    private readonly List<Tour> _allTours = new();

    public void AddTourNeedingReplacement(Tour tour)
    {
        _toursNeedingReplacement.Add(tour);
        _allTours.Add(tour);
    }

    public Task<IEnumerable<Tour>> GetToursStartingWithin24HoursNeedingReplacementAsync()
    {
        return Task.FromResult(_toursNeedingReplacement.AsEnumerable());
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }

    // Other interface methods - not used in these tests
    public Task AddAsync(Tour tour)
    {
        _allTours.Add(tour);
        return Task.CompletedTask;
    }

    public Task<Tour?> GetByIdAsync(Guid id)
        => Task.FromResult(_allTours.FirstOrDefault(t => t.Id == id));

    public Task<IEnumerable<Tour>> GetByGuideAsync(Guid guideId, bool sortAscending = true)
        => Task.FromResult(_allTours.Where(t => t.GuideId == guideId));

    public Task<IEnumerable<Tour>> GetPublishedAsync(bool sortAscending = true)
        => Task.FromResult(_allTours.Where(t => t.Status == TourStatus.Published));

    public Task<KeyPoint> AddKeyPointAsync(Tour tour, double latitude, double longitude, string name, string description, string imageUrl)
        => throw new NotImplementedException();

    public Task<IEnumerable<Tour>> GetToursNeedingReplacementAsync(Guid excludeGuideId)
        => Task.FromResult(_toursNeedingReplacement.Where(t => t.GuideId != excludeGuideId));

    public Task<IEnumerable<Tour>> GetToursStartingIn48HoursAsync()
        => Task.FromResult(Enumerable.Empty<Tour>());

    public Task<bool> HasTourOnDateAsync(Guid guideId, DateTime date)
        => Task.FromResult(false);
}

public class CancellationFakeEmailService : IEmailService
{
    public int CancellationEmailsSent { get; private set; }

    public Task SendPurchaseConfirmationAsync(string toEmail, string touristName, PurchaseResultDto purchase)
        => Task.CompletedTask;

    public Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime startDate, string description)
        => Task.CompletedTask;

    public Task SendTourCancellationAsync(string toEmail, string touristName, string tourName, DateTime startDate, int bonusPointsAwarded)
    {
        CancellationEmailsSent++;
        return Task.CompletedTask;
    }

    public Task SendProblemNotificationToGuideAsync(Guid guideId, string tourName, string problemTitle)
        => Task.CompletedTask;
}

#endregion
