using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TourApp.Application.Cart.DTOs;
using TourApp.Application.Cart.Services;
using TourApp.Application.Purchases.DTOs;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Application.Purchases.Services;
using TourApp.Domain.Purchases;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tests;

public class PurchaseServiceTests : IDisposable
{
    private readonly PurchaseTestDbContext _dbContext;
    private readonly FakeCartService _cartService;
    private readonly FakePurchaseRepository _purchaseRepository;
    private readonly FakeEmailService _emailService;
    private readonly PurchaseService _purchaseService;
    private readonly Guid _touristId = Guid.NewGuid();
    private readonly Guid _tourId = Guid.NewGuid();

    public PurchaseServiceTests()
    {
        var options = new DbContextOptionsBuilder<PurchaseTestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new PurchaseTestDbContext(options);
        _cartService = new FakeCartService();
        _purchaseRepository = new FakePurchaseRepository();
        _emailService = new FakeEmailService();

        _purchaseService = new PurchaseService(
            _purchaseRepository,
            _cartService,
            _emailService,
            _dbContext);

        SeedData();
    }

    private void SeedData()
    {
        var tour = new Tour(
            _tourId,
            Guid.NewGuid(),
            "Test Tour",
            "Test Description",
            Difficulty.Easy,
            Interest.Nature,
            100.00m,
            DateTime.UtcNow.AddDays(30));

        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();

        _dbContext.Tours.Add(tour);

        var tourist = new Tourist(
            _touristId,
            "testuser",
            "test@example.com",
            "John",
            "Doe",
            "hashedpassword123",
            true,
            new[] { Interest.Nature });

        _dbContext.Tourists.Add(tourist);
        _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    #region ConfirmPurchase - Bonus Points Tests

    [Fact]
    public async Task ConfirmPurchase_WithUseBonus_FinalPriceNeverGoesBelowZero()
    {
        // Arrange
        var tourist = await _dbContext.Tourists.FirstAsync(t => t.Id == _touristId);
        tourist.AddBonusPoints(200); // More bonus points than price (100)
        await _dbContext.SaveChangesAsync();

        _cartService.SetCartItems(_touristId, new List<CartItemDto>
        {
            new() { TourId = _tourId, Name = "Test Tour", Price = 100.00m }
        });

        // Act
        var result = await _purchaseService.ConfirmPurchaseAsync(_touristId, useBonus: true);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.FinalPrice.Should().BeGreaterThanOrEqualTo(0);
        result.Data.FinalPrice.Should().Be(0); // 100 - 100 = 0 (capped at price)
    }

    [Fact]
    public async Task ConfirmPurchase_WithUseBonus_BonusPointsReducedByExactAmountUsed()
    {
        // Arrange
        var tourist = await _dbContext.Tourists.FirstAsync(t => t.Id == _touristId);
        tourist.AddBonusPoints(50); // Less bonus points than price
        await _dbContext.SaveChangesAsync();

        _cartService.SetCartItems(_touristId, new List<CartItemDto>
        {
            new() { TourId = _tourId, Name = "Test Tour", Price = 100.00m }
        });

        // Act
        var result = await _purchaseService.ConfirmPurchaseAsync(_touristId, useBonus: true);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.BonusPointsUsed.Should().Be(50);
        result.Data.FinalPrice.Should().Be(50); // 100 - 50 = 50
    }

    [Fact]
    public async Task ConfirmPurchase_WithUseBonus_RemainingBonusPointsArePreserved()
    {
        // Arrange
        var tourist = await _dbContext.Tourists.FirstAsync(t => t.Id == _touristId);
        tourist.AddBonusPoints(150); // 150 bonus points, tour costs 100
        await _dbContext.SaveChangesAsync();

        _cartService.SetCartItems(_touristId, new List<CartItemDto>
        {
            new() { TourId = _tourId, Name = "Test Tour", Price = 100.00m }
        });

        // Act
        var result = await _purchaseService.ConfirmPurchaseAsync(_touristId, useBonus: true);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.BonusPointsUsed.Should().Be(100); // Only 100 used (tour price)

        // Refresh tourist from database
        var updatedTourist = await _dbContext.Tourists.FirstAsync(t => t.Id == _touristId);
        // 150 - 100 (used) + 10 (earned per purchase) = 60
        updatedTourist.BonusPoints.Should().Be(60);
    }

    [Fact]
    public async Task ConfirmPurchase_WithoutUseBonus_BonusPointsNotUsed()
    {
        // Arrange
        var tourist = await _dbContext.Tourists.FirstAsync(t => t.Id == _touristId);
        tourist.AddBonusPoints(100);
        await _dbContext.SaveChangesAsync();

        _cartService.SetCartItems(_touristId, new List<CartItemDto>
        {
            new() { TourId = _tourId, Name = "Test Tour", Price = 100.00m }
        });

        // Act
        var result = await _purchaseService.ConfirmPurchaseAsync(_touristId, useBonus: false);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.BonusPointsUsed.Should().Be(0);
        result.Data.FinalPrice.Should().Be(100.00m);

        // Refresh tourist - should still have 100 + 10 earned = 110
        var updatedTourist = await _dbContext.Tourists.FirstAsync(t => t.Id == _touristId);
        updatedTourist.BonusPoints.Should().Be(110);
    }

    [Fact]
    public async Task ConfirmPurchase_WithZeroBonusPoints_FinalPriceEqualsOriginal()
    {
        // Arrange - tourist has 0 bonus points by default
        _cartService.SetCartItems(_touristId, new List<CartItemDto>
        {
            new() { TourId = _tourId, Name = "Test Tour", Price = 100.00m }
        });

        // Act
        var result = await _purchaseService.ConfirmPurchaseAsync(_touristId, useBonus: true);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.BonusPointsUsed.Should().Be(0);
        result.Data.FinalPrice.Should().Be(100.00m);
        result.Data.OriginalPrice.Should().Be(100.00m);
    }

    [Fact]
    public async Task ConfirmPurchase_EmptyCart_ReturnsError()
    {
        // Arrange - empty cart
        _cartService.SetCartItems(_touristId, new List<CartItemDto>());

        // Act
        var result = await _purchaseService.ConfirmPurchaseAsync(_touristId, useBonus: false);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("CART_EMPTY");
    }

    #endregion
}

#region Fake Implementations

public class PurchaseTestDbContext : DbContext
{
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<Tourist> Tourists { get; set; } = null!;
    public DbSet<KeyPoint> KeyPoints { get; set; } = null!;
    public DbSet<TouristInterest> TouristInterests { get; set; } = null!;

    public PurchaseTestDbContext(DbContextOptions<PurchaseTestDbContext> options) : base(options)
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
    }
}

public class FakeCartService : ICartService
{
    private readonly Dictionary<Guid, List<CartItemDto>> _carts = new();

    public void SetCartItems(Guid touristId, List<CartItemDto> items)
    {
        _carts[touristId] = items;
    }

    public List<CartItemDto> GetCartItems(Guid touristId)
    {
        return _carts.TryGetValue(touristId, out var items) ? items : new List<CartItemDto>();
    }

    public void ClearCart(Guid touristId)
    {
        _carts.Remove(touristId);
    }

    public Task<TourApp.Shared.ApiResponse<CartDto>> AddToCartAsync(Guid touristId, Guid tourId)
        => throw new NotImplementedException();

    public Task<TourApp.Shared.ApiResponse<CartDto>> RemoveFromCartAsync(Guid touristId, Guid tourId)
        => throw new NotImplementedException();

    public Task<TourApp.Shared.ApiResponse<CartDto>> GetCartAsync(Guid touristId)
        => throw new NotImplementedException();
}

public class FakePurchaseRepository : IPurchaseRepository
{
    private readonly List<Purchase> _purchases = new();

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

public class FakeEmailService : IEmailService
{
    public Task SendPurchaseConfirmationAsync(string toEmail, string touristName, PurchaseResultDto purchase)
        => Task.CompletedTask;

    public Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime startDate, string description)
        => Task.CompletedTask;

    public Task SendTourCancellationAsync(string toEmail, string touristName, string tourName, DateTime startDate, int bonusPointsAwarded)
        => Task.CompletedTask;

    public Task SendProblemNotificationToGuideAsync(Guid guideId, string tourName, string problemTitle)
        => Task.CompletedTask;
}

#endregion
