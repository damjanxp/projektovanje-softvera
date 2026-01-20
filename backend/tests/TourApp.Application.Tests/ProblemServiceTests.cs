using FluentAssertions;
using TourApp.Application.Problems.DTOs;
using TourApp.Application.Problems.Interfaces;
using TourApp.Application.Problems.Services;
using TourApp.Application.Purchases.DTOs;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Application.Purchases.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Domain.Problems;
using TourApp.Domain.Purchases;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tests;

public class ProblemServiceTests
{
    private readonly FakeProblemRepository _problemRepository;
    private readonly FakeProblemPurchaseRepository _purchaseRepository;
    private readonly FakeProblemTourRepository _tourRepository;
    private readonly FakeProblemEmailService _emailService;
    private readonly ProblemService _problemService;

    private readonly Guid _touristId = Guid.NewGuid();
    private readonly Guid _guideId = Guid.NewGuid();
    private readonly Guid _adminId = Guid.NewGuid();

    public ProblemServiceTests()
    {
        _problemRepository = new FakeProblemRepository();
        _purchaseRepository = new FakeProblemPurchaseRepository();
        _tourRepository = new FakeProblemTourRepository();
        _emailService = new FakeProblemEmailService();

        _problemService = new ProblemService(
            _problemRepository,
            _purchaseRepository,
            _tourRepository,
            _emailService);
    }

    #region Create Problem - Purchase Validation Tests

    [Fact]
    public async Task CreateProblem_WhenTouristHasNotPurchasedTour_ReturnsNotPurchasedError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        // Tourist has no purchases
        var request = new CreateProblemRequest
        {
            TourId = tourId,
            Title = "Problem Title",
            Description = "Problem Description"
        };

        // Act
        var result = await _problemService.CreateProblemAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("NOT_PURCHASED");
        result.Error.Message.Should().Be("You can only report problems for tours you have purchased.");
    }

    [Fact]
    public async Task CreateProblem_WhenTouristPurchasedDifferentTour_ReturnsNotPurchasedError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var differentTourId = Guid.NewGuid();

        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        // Tourist purchased a different tour
        var purchase = CreatePurchase(_touristId, differentTourId, "Different Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateProblemRequest
        {
            TourId = tourId,
            Title = "Problem Title",
            Description = "Problem Description"
        };

        // Act
        var result = await _problemService.CreateProblemAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("NOT_PURCHASED");
    }

    [Fact]
    public async Task CreateProblem_WhenTouristHasPurchasedTour_Succeeds()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var purchase = CreatePurchase(_touristId, tourId, "Test Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateProblemRequest
        {
            TourId = tourId,
            Title = "Problem Title",
            Description = "Problem Description"
        };

        // Act
        var result = await _problemService.CreateProblemAsync(_touristId, request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Title.Should().Be("Problem Title");
        result.Data.Status.Should().Be(ProblemStatus.Pending);
    }

    [Fact]
    public async Task CreateProblem_WhenTourNotFound_ReturnsTourNotFoundError()
    {
        // Arrange
        var nonExistentTourId = Guid.NewGuid();

        var request = new CreateProblemRequest
        {
            TourId = nonExistentTourId,
            Title = "Problem Title",
            Description = "Problem Description"
        };

        // Act
        var result = await _problemService.CreateProblemAsync(_touristId, request);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("TOUR_NOT_FOUND");
    }

    #endregion

    #region Status Change - Event Storage Tests

    [Fact]
    public async Task ResolveProblem_WhenSuccessful_EventIsStoredInRepository()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var problem = new Problem(
            Guid.NewGuid(),
            _touristId,
            tourId,
            "Test Problem",
            "Test Description");
        _problemRepository.AddProblem(problem);

        // Act
        var result = await _problemService.ResolveProblemAsync(problem.Id, _guideId);

        // Assert
        result.Success.Should().BeTrue();
        _problemRepository.StoredEvents.Should().HaveCount(1);

        var storedEvent = _problemRepository.StoredEvents.First();
        storedEvent.ProblemId.Should().Be(problem.Id);
        storedEvent.OldStatus.Should().Be(ProblemStatus.Pending);
        storedEvent.NewStatus.Should().Be(ProblemStatus.Resolved);
        storedEvent.ChangedByRole.Should().Be("Guide");
        storedEvent.ChangedByUserId.Should().Be(_guideId);
    }

    [Fact]
    public async Task SendToReview_WhenSuccessful_EventIsStoredInRepository()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var problem = new Problem(
            Guid.NewGuid(),
            _touristId,
            tourId,
            "Test Problem",
            "Test Description");
        _problemRepository.AddProblem(problem);

        // Act
        var result = await _problemService.SendToReviewAsync(problem.Id, _guideId);

        // Assert
        result.Success.Should().BeTrue();
        _problemRepository.StoredEvents.Should().HaveCount(1);

        var storedEvent = _problemRepository.StoredEvents.First();
        storedEvent.ProblemId.Should().Be(problem.Id);
        storedEvent.OldStatus.Should().Be(ProblemStatus.Pending);
        storedEvent.NewStatus.Should().Be(ProblemStatus.InReview);
    }

    [Fact]
    public async Task RejectProblem_WhenSuccessful_EventIsStoredInRepository()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var problem = new Problem(
            Guid.NewGuid(),
            _touristId,
            tourId,
            "Test Problem",
            "Test Description");
        problem.SendToReview(_guideId); // Move to InReview first
        _problemRepository.AddProblem(problem);

        // Act
        var result = await _problemService.RejectProblemAsync(problem.Id, _adminId);

        // Assert
        result.Success.Should().BeTrue();
        _problemRepository.StoredEvents.Should().HaveCount(1);

        var storedEvent = _problemRepository.StoredEvents.First();
        storedEvent.ProblemId.Should().Be(problem.Id);
        storedEvent.OldStatus.Should().Be(ProblemStatus.InReview);
        storedEvent.NewStatus.Should().Be(ProblemStatus.Rejected);
        storedEvent.ChangedByRole.Should().Be("Admin");
    }

    [Fact]
    public async Task ReopenProblem_WhenSuccessful_EventIsStoredInRepository()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var problem = new Problem(
            Guid.NewGuid(),
            _touristId,
            tourId,
            "Test Problem",
            "Test Description");
        problem.SendToReview(_guideId); // Move to InReview first
        _problemRepository.AddProblem(problem);

        // Act
        var result = await _problemService.ReopenProblemAsync(problem.Id, _adminId);

        // Assert
        result.Success.Should().BeTrue();
        _problemRepository.StoredEvents.Should().HaveCount(1);

        var storedEvent = _problemRepository.StoredEvents.First();
        storedEvent.ProblemId.Should().Be(problem.Id);
        storedEvent.OldStatus.Should().Be(ProblemStatus.InReview);
        storedEvent.NewStatus.Should().Be(ProblemStatus.Pending);
    }

    [Fact]
    public async Task MultipleStatusChanges_AllEventsAreStored()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var problem = new Problem(
            Guid.NewGuid(),
            _touristId,
            tourId,
            "Test Problem",
            "Test Description");
        _problemRepository.AddProblem(problem);

        // Act
        await _problemService.SendToReviewAsync(problem.Id, _guideId);
        await _problemService.ReopenProblemAsync(problem.Id, _adminId);
        await _problemService.ResolveProblemAsync(problem.Id, _guideId);

        // Assert
        _problemRepository.StoredEvents.Should().HaveCount(3);
    }

    #endregion

    #region Authorization Tests

    [Fact]
    public async Task ResolveProblem_WhenGuideDoesNotOwnTour_ReturnsUnauthorizedError()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var differentGuideId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId); // Owned by _guideId
        _tourRepository.AddTour(tour);

        var problem = new Problem(
            Guid.NewGuid(),
            _touristId,
            tourId,
            "Test Problem",
            "Test Description");
        _problemRepository.AddProblem(problem);

        // Act - different guide tries to resolve
        var result = await _problemService.ResolveProblemAsync(problem.Id, differentGuideId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("UNAUTHORIZED");
    }

    [Fact]
    public async Task ResolveProblem_WhenProblemNotFound_ReturnsProblemNotFoundError()
    {
        // Arrange
        var nonExistentProblemId = Guid.NewGuid();

        // Act
        var result = await _problemService.ResolveProblemAsync(nonExistentProblemId, _guideId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("PROBLEM_NOT_FOUND");
    }

    #endregion

    #region Email Notification Tests

    [Fact]
    public async Task CreateProblem_WhenSuccessful_SendsEmailToGuide()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tour = CreatePublishedTour(tourId);
        _tourRepository.AddTour(tour);

        var purchase = CreatePurchase(_touristId, tourId, "Test Tour", 100m);
        _purchaseRepository.AddPurchase(purchase);

        var request = new CreateProblemRequest
        {
            TourId = tourId,
            Title = "Problem Title",
            Description = "Problem Description"
        };

        // Act
        await _problemService.CreateProblemAsync(_touristId, request);

        // Assert
        _emailService.ProblemNotificationsSent.Should().Be(1);
    }

    #endregion

    #region Helper Methods

    private Tour CreatePublishedTour(Guid tourId)
    {
        var tour = new Tour(
            tourId,
            _guideId,
            "Test Tour",
            "Test Description",
            Difficulty.Easy,
            Interest.Nature,
            100.00m,
            DateTime.UtcNow.AddDays(-3));

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

#region Fake Repositories for Problem Tests

public class FakeProblemRepository : IProblemRepository
{
    private readonly List<Problem> _problems = new();
    private readonly List<ProblemStatusChangedEvent> _events = new();

    public List<ProblemStatusChangedEvent> StoredEvents => _events;

    public void AddProblem(Problem problem)
    {
        _problems.Add(problem);
    }

    public Task AddAsync(Problem problem)
    {
        _problems.Add(problem);
        return Task.CompletedTask;
    }

    public Task<Problem?> GetByIdAsync(Guid id)
    {
        return Task.FromResult(_problems.FirstOrDefault(p => p.Id == id));
    }

    public Task<Problem?> GetByIdWithEventsAsync(Guid id)
    {
        return Task.FromResult(_problems.FirstOrDefault(p => p.Id == id));
    }

    public Task<IEnumerable<Problem>> GetByTourAsync(Guid tourId)
    {
        return Task.FromResult(_problems.Where(p => p.TourId == tourId));
    }

    public Task<IEnumerable<Problem>> GetByTouristAsync(Guid touristId)
    {
        return Task.FromResult(_problems.Where(p => p.TouristId == touristId));
    }

    public Task<IEnumerable<Problem>> GetProblemsInReviewAsync()
    {
        return Task.FromResult(_problems.Where(p => p.Status == ProblemStatus.InReview));
    }

    public Task AddEventAsync(ProblemStatusChangedEvent statusEvent)
    {
        _events.Add(statusEvent);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<ProblemStatusChangedEvent>> GetEventsByProblemAsync(Guid problemId)
    {
        return Task.FromResult(_events.Where(e => e.ProblemId == problemId));
    }

    public Task SaveChangesAsync()
    {
        return Task.CompletedTask;
    }
}

public class FakeProblemPurchaseRepository : IPurchaseRepository
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

public class FakeProblemTourRepository : ITourRepository
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

public class FakeProblemEmailService : IEmailService
{
    public int ProblemNotificationsSent { get; private set; }

    public Task SendPurchaseConfirmationAsync(string toEmail, string touristName, PurchaseResultDto purchase)
        => Task.CompletedTask;

    public Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime startDate, string description)
        => Task.CompletedTask;

    public Task SendTourCancellationAsync(string toEmail, string touristName, string tourName, DateTime startDate, int bonusPointsAwarded)
        => Task.CompletedTask;

    public Task SendProblemNotificationToGuideAsync(Guid guideId, string tourName, string problemTitle)
    {
        ProblemNotificationsSent++;
        return Task.CompletedTask;
    }
}

#endregion
