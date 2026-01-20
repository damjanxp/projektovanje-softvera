using FluentAssertions;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Domain.Tests.Tours;

public class TourPublishingTests
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

    [Fact]
    public void Publish_WithLessThanTwoKeyPoints_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");

        // Act
        var act = () => tour.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tour must have at least 2 key points before publishing.");
    }

    [Fact]
    public void Publish_WithNoKeyPoints_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = CreateValidTour();

        // Act
        var act = () => tour.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tour must have at least 2 key points before publishing.");
    }

    [Fact]
    public void Publish_WithTwoKeyPoints_SucceedsAndStatusBecomesPublished()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");

        // Act
        tour.Publish();

        // Assert
        tour.Status.Should().Be(TourStatus.Published);
    }

    [Fact]
    public void Publish_WithMoreThanTwoKeyPoints_SucceedsAndStatusBecomesPublished()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.AddKeyPoint(47.0, 17.0, "Point 3", "Description 3", "https://example.com/image3.jpg");

        // Act
        tour.Publish();

        // Assert
        tour.Status.Should().Be(TourStatus.Published);
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Publish();

        // Act
        var act = () => tour.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Tour is already published.");
    }

    [Fact]
    public void Publish_WhenCanceled_ThrowsInvalidOperationException()
    {
        // Arrange
        var tour = CreateValidTour();
        tour.AddKeyPoint(45.0, 15.0, "Point 1", "Description 1", "https://example.com/image1.jpg");
        tour.AddKeyPoint(46.0, 16.0, "Point 2", "Description 2", "https://example.com/image2.jpg");
        tour.Cancel();

        // Act
        var act = () => tour.Publish();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot publish a canceled tour.");
    }
}
