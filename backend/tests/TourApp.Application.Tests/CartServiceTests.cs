using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TourApp.Application.Cart.Services;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Application.Tests;

public class CartServiceTests : IDisposable
{
    private readonly TestDbContext _dbContext;
    private readonly CartService _cartService;
    private readonly Guid _touristId = Guid.NewGuid();
    private readonly Guid _tourId = Guid.NewGuid();

    public CartServiceTests()
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new TestDbContext(options);
        _cartService = new CartService(_dbContext);

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

    #region AddToCart Tests

    [Fact]
    public async Task AddToCart_WhenTourNotInCart_AddsSuccessfully()
    {
        // Act
        var result = await _cartService.AddToCartAsync(_touristId, _tourId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().TourId.Should().Be(_tourId);
    }

    [Fact]
    public async Task AddToCart_WhenTourAlreadyInCart_ReturnsDuplicateError()
    {
        // Arrange
        await _cartService.AddToCartAsync(_touristId, _tourId);

        // Act
        var result = await _cartService.AddToCartAsync(_touristId, _tourId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("TOUR_ALREADY_IN_CART");
        result.Error.Message.Should().Be("This tour is already in your cart.");
    }

    [Fact]
    public async Task AddToCart_WhenTourNotFound_ReturnsNotFoundError()
    {
        // Act
        var result = await _cartService.AddToCartAsync(_touristId, Guid.NewGuid());

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("TOUR_NOT_FOUND");
    }

    [Fact]
    public async Task AddToCart_WhenTourNotPublished_ReturnsError()
    {
        // Arrange
        var draftTourId = Guid.NewGuid();
        var draftTour = new Tour(
            draftTourId,
            Guid.NewGuid(),
            "Draft Tour",
            "Draft Description",
            Difficulty.Easy,
            Interest.Art,
            50.00m,
            DateTime.UtcNow.AddDays(60));

        _dbContext.Tours.Add(draftTour);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _cartService.AddToCartAsync(_touristId, draftTourId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("TOUR_NOT_PUBLISHED");
    }

    #endregion

    #region RemoveFromCart Tests

    [Fact]
    public async Task RemoveFromCart_WhenTourInCart_RemovesSuccessfully()
    {
        // Arrange
        await _cartService.AddToCartAsync(_touristId, _tourId);

        // Act
        var result = await _cartService.RemoveFromCartAsync(_touristId, _tourId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task RemoveFromCart_WhenCartEmpty_ReturnsError()
    {
        // Act
        var result = await _cartService.RemoveFromCartAsync(_touristId, _tourId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("CART_EMPTY");
    }

    [Fact]
    public async Task RemoveFromCart_WhenTourNotInCart_ReturnsError()
    {
        // Arrange
        await _cartService.AddToCartAsync(_touristId, _tourId);

        // Act
        var result = await _cartService.RemoveFromCartAsync(_touristId, Guid.NewGuid());

        // Assert
        result.Success.Should().BeFalse();
        result.Error!.Code.Should().Be("TOUR_NOT_IN_CART");
    }

    [Fact]
    public async Task RemoveFromCart_WhenMultipleItemsInCart_RemovesOnlySpecifiedItem()
    {
        // Arrange
        var secondTourId = Guid.NewGuid();
        var secondTour = new Tour(
            secondTourId,
            Guid.NewGuid(),
            "Second Tour",
            "Second Description",
            Difficulty.Medium,
            Interest.Food,
            75.00m,
            DateTime.UtcNow.AddDays(45));

        secondTour.AddKeyPoint(47.0, 17.0, "Point A", "Description A", "https://example.com/imageA.jpg");
        secondTour.AddKeyPoint(48.0, 18.0, "Point B", "Description B", "https://example.com/imageB.jpg");
        secondTour.Publish();

        _dbContext.Tours.Add(secondTour);
        await _dbContext.SaveChangesAsync();

        await _cartService.AddToCartAsync(_touristId, _tourId);
        await _cartService.AddToCartAsync(_touristId, secondTourId);

        // Act
        var result = await _cartService.RemoveFromCartAsync(_touristId, _tourId);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Items.Should().HaveCount(1);
        result.Data.Items.First().TourId.Should().Be(secondTourId);
    }

    #endregion
}

public class TestDbContext : DbContext
{
    public DbSet<Tour> Tours { get; set; } = null!;
    public DbSet<Tourist> Tourists { get; set; } = null!;
    public DbSet<KeyPoint> KeyPoints { get; set; } = null!;
    public DbSet<TouristInterest> TouristInterests { get; set; } = null!;

    public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
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
