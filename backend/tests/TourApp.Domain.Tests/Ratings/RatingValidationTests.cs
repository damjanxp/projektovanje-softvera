using FluentAssertions;
using TourApp.Domain.Ratings;

namespace TourApp.Domain.Tests.Ratings;

public class RatingValidationTests
{
    private static DateTime GetValidTourStartDate() => DateTime.UtcNow.AddDays(-3);

    #region Score Validation Tests

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Rating_WithScoreLessThan1_ThrowsArgumentOutOfRangeException(int invalidScore)
    {
        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            invalidScore,
            "Some comment",
            GetValidTourStartDate());

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Score must be between 1 and 5*");
    }

    [Theory]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(100)]
    public void Rating_WithScoreGreaterThan5_ThrowsArgumentOutOfRangeException(int invalidScore)
    {
        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            invalidScore,
            "Some comment",
            GetValidTourStartDate());

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("*Score must be between 1 and 5*");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Rating_WithValidScore_Succeeds(int validScore)
    {
        // Act
        var rating = new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            validScore,
            "This is a comment for low scores",
            GetValidTourStartDate());

        // Assert
        rating.Score.Should().Be(validScore);
    }

    #endregion

    #region Low Score Comment Validation Tests

    [Fact]
    public void Rating_WithScore1_WithoutComment_ThrowsArgumentException()
    {
        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            null,
            GetValidTourStartDate());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*A comment is required for ratings of 1 or 2*");
    }

    [Fact]
    public void Rating_WithScore1_WithEmptyComment_ThrowsArgumentException()
    {
        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            "   ",
            GetValidTourStartDate());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*A comment is required for ratings of 1 or 2*");
    }

    [Fact]
    public void Rating_WithScore2_WithoutComment_ThrowsArgumentException()
    {
        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            null,
            GetValidTourStartDate());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*A comment is required for ratings of 1 or 2*");
    }

    [Fact]
    public void Rating_WithScore2_WithEmptyComment_ThrowsArgumentException()
    {
        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            "",
            GetValidTourStartDate());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*A comment is required for ratings of 1 or 2*");
    }

    [Fact]
    public void Rating_WithScore3_WithoutComment_Succeeds()
    {
        // Act
        var rating = new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            3,
            null,
            GetValidTourStartDate());

        // Assert
        rating.Score.Should().Be(3);
        rating.Comment.Should().BeNull();
    }

    [Fact]
    public void Rating_WithScore4_WithoutComment_Succeeds()
    {
        // Act
        var rating = new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            4,
            null,
            GetValidTourStartDate());

        // Assert
        rating.Score.Should().Be(4);
        rating.Comment.Should().BeNull();
    }

    [Fact]
    public void Rating_WithScore5_WithoutComment_Succeeds()
    {
        // Act
        var rating = new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            null,
            GetValidTourStartDate());

        // Assert
        rating.Score.Should().Be(5);
        rating.Comment.Should().BeNull();
    }

    [Fact]
    public void Rating_WithScore1_WithValidComment_Succeeds()
    {
        // Arrange
        var comment = "The tour was very disappointing.";

        // Act
        var rating = new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            comment,
            GetValidTourStartDate());

        // Assert
        rating.Score.Should().Be(1);
        rating.Comment.Should().Be(comment);
    }

    [Fact]
    public void Rating_WithScore2_WithValidComment_Succeeds()
    {
        // Arrange
        var comment = "Below expectations, needs improvement.";

        // Act
        var rating = new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            2,
            comment,
            GetValidTourStartDate());

        // Assert
        rating.Score.Should().Be(2);
        rating.Comment.Should().Be(comment);
    }

    #endregion

    #region Tour Date Validation Tests

    [Fact]
    public void Rating_BeforeTourTakesPlace_ThrowsInvalidOperationException()
    {
        // Arrange - tour is in the future
        var futureTourDate = DateTime.UtcNow.AddDays(5);

        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            null,
            futureTourDate);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot rate a tour before it has taken place*");
    }

    [Fact]
    public void Rating_MoreThan7DaysAfterTour_ThrowsInvalidOperationException()
    {
        // Arrange - tour was more than 7 days ago
        var oldTourDate = DateTime.UtcNow.AddDays(-10);

        // Act
        var act = () => new Rating(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            5,
            null,
            oldTourDate);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot rate a tour more than 7 days after it has taken place*");
    }

    #endregion
}
