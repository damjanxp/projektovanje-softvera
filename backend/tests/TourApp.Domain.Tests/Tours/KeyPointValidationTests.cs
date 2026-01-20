using FluentAssertions;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Domain.Tests.Tours;

public class KeyPointValidationTests
{
    private static Tour CreateValidTour()
    {
        return new Tour(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Tour",
            "Test Description",
            Difficulty.Easy,
            Interest.Nature,
            100.00m,
            DateTime.UtcNow.AddDays(30));
    }

    #region Latitude Validation Tests

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    [InlineData(-100)]
    [InlineData(100)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public void AddKeyPoint_WithInvalidLatitude_ThrowsArgumentOutOfRangeException(double invalidLatitude)
    {
        // Arrange
        var tour = CreateValidTour();

        // Act
        var act = () => tour.AddKeyPoint(invalidLatitude, 15.0, "Point", "Description", "https://example.com/image.jpg");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Latitude must be between -90 and 90*");
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(90)]
    [InlineData(0)]
    [InlineData(45.5)]
    [InlineData(-45.5)]
    public void AddKeyPoint_WithValidLatitude_Succeeds(double validLatitude)
    {
        // Arrange
        var tour = CreateValidTour();

        // Act
        tour.AddKeyPoint(validLatitude, 15.0, "Point", "Description", "https://example.com/image.jpg");

        // Assert
        tour.KeyPoints.Should().HaveCount(1);
        tour.KeyPoints.First().Latitude.Should().Be(validLatitude);
    }

    #endregion

    #region Longitude Validation Tests

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    [InlineData(-200)]
    [InlineData(200)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public void AddKeyPoint_WithInvalidLongitude_ThrowsArgumentOutOfRangeException(double invalidLongitude)
    {
        // Arrange
        var tour = CreateValidTour();

        // Act
        var act = () => tour.AddKeyPoint(45.0, invalidLongitude, "Point", "Description", "https://example.com/image.jpg");

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Longitude must be between -180 and 180*");
    }

    [Theory]
    [InlineData(-180)]
    [InlineData(180)]
    [InlineData(0)]
    [InlineData(90.5)]
    [InlineData(-90.5)]
    public void AddKeyPoint_WithValidLongitude_Succeeds(double validLongitude)
    {
        // Arrange
        var tour = CreateValidTour();

        // Act
        tour.AddKeyPoint(45.0, validLongitude, "Point", "Description", "https://example.com/image.jpg");

        // Assert
        tour.KeyPoints.Should().HaveCount(1);
        tour.KeyPoints.First().Longitude.Should().Be(validLongitude);
    }

    #endregion

    #region KeyPoint Constructor Direct Tests

    [Fact]
    public void KeyPoint_WithLatitudeBelow90_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => new KeyPoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            -91,
            15.0,
            "Point",
            "Description",
            "https://example.com/image.jpg",
            0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("latitude");
    }

    [Fact]
    public void KeyPoint_WithLatitudeAbove90_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => new KeyPoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            91,
            15.0,
            "Point",
            "Description",
            "https://example.com/image.jpg",
            0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("latitude");
    }

    [Fact]
    public void KeyPoint_WithLongitudeBelow180_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => new KeyPoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            45.0,
            -181,
            "Point",
            "Description",
            "https://example.com/image.jpg",
            0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("longitude");
    }

    [Fact]
    public void KeyPoint_WithLongitudeAbove180_ThrowsArgumentOutOfRangeException()
    {
        // Act
        var act = () => new KeyPoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            45.0,
            181,
            "Point",
            "Description",
            "https://example.com/image.jpg",
            0);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("longitude");
    }

    [Fact]
    public void KeyPoint_WithValidBoundaryCoordinates_Succeeds()
    {
        // Act
        var keyPoint = new KeyPoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            90,
            180,
            "Point",
            "Description",
            "https://example.com/image.jpg",
            0);

        // Assert
        keyPoint.Latitude.Should().Be(90);
        keyPoint.Longitude.Should().Be(180);
    }

    [Fact]
    public void KeyPoint_WithNegativeBoundaryCoordinates_Succeeds()
    {
        // Act
        var keyPoint = new KeyPoint(
            Guid.NewGuid(),
            Guid.NewGuid(),
            -90,
            -180,
            "Point",
            "Description",
            "https://example.com/image.jpg",
            0);

        // Assert
        keyPoint.Latitude.Should().Be(-90);
        keyPoint.Longitude.Should().Be(-180);
    }

    #endregion

    #region Tour Status Restrictions

    [Fact]
    public void AddKeyPoint_WhenTourIsPublished_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();

        // Act
        var act = () => tour.AddKeyPoint(47.0, 17.0, "Point 3", "Description 3", "https://example.com/image3.jpg");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add key points to a published tour.");
    }

    [Fact]
    public void AddKeyPoint_WhenTourIsCanceled_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.Cancel();

        // Act
        var act = () => tour.AddKeyPoint(45.0, 15.0, "Point", "Description", "https://example.com/image.jpg");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot add key points to a canceled tour.");
    }

    #endregion
}
