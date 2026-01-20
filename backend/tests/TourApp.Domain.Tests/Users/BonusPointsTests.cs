using FluentAssertions;
using TourApp.Domain.Users;

namespace TourApp.Domain.Tests.Users;

public class BonusPointsTests
{
    private static Tourist CreateValidTourist()
    {
        return new Tourist(
            Guid.NewGuid(),
            "testuser",
            "test@example.com",
            "John",
            "Doe",
            "hashedpassword123",
            true,
            new[] { Interest.Nature, Interest.Art });
    }

    #region AddBonusPoints Tests

    [Fact]
    public void AddBonusPoints_WithPositivePoints_IncreasesBonusPoints()
    {
        // Arrange
        var tourist = CreateValidTourist();

        // Act
        tourist.AddBonusPoints(100);

        // Assert
        tourist.BonusPoints.Should().Be(100);
    }

    [Fact]
    public void AddBonusPoints_MultipleAdditions_AccumulatesCorrectly()
    {
        // Arrange
        var tourist = CreateValidTourist();

        // Act
        tourist.AddBonusPoints(50);
        tourist.AddBonusPoints(30);
        tourist.AddBonusPoints(20);

        // Assert
        tourist.BonusPoints.Should().Be(100);
    }

    [Fact]
    public void AddBonusPoints_WithZero_BonusPointsRemainUnchanged()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(50);

        // Act
        tourist.AddBonusPoints(0);

        // Assert
        tourist.BonusPoints.Should().Be(50);
    }

    [Fact]
    public void AddBonusPoints_WithNegativePoints_ThrowsArgumentException()
    {
        // Arrange
        var tourist = CreateValidTourist();

        // Act
        var act = () => tourist.AddBonusPoints(-10);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot add negative bonus points.*");
    }

    #endregion

    #region SpendBonusPoints Tests

    [Fact]
    public void SpendBonusPoints_WithValidAmount_DecreasesBonusPoints()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(100);

        // Act
        tourist.SpendBonusPoints(30);

        // Assert
        tourist.BonusPoints.Should().Be(70);
    }

    [Fact]
    public void SpendBonusPoints_ExactBalance_ResultsInZeroBonusPoints()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(50);

        // Act
        tourist.SpendBonusPoints(50);

        // Assert
        tourist.BonusPoints.Should().Be(0);
    }

    [Fact]
    public void SpendBonusPoints_MoreThanAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(50);

        // Act
        var act = () => tourist.SpendBonusPoints(100);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot spend 100 bonus points. Available: 50.");
    }

    [Fact]
    public void SpendBonusPoints_WithZeroBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var tourist = CreateValidTourist();

        // Act
        var act = () => tourist.SpendBonusPoints(10);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot spend 10 bonus points. Available: 0.");
    }

    [Fact]
    public void SpendBonusPoints_WithNegativePoints_ThrowsArgumentException()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(100);

        // Act
        var act = () => tourist.SpendBonusPoints(-10);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("Cannot spend negative bonus points.*");
    }

    [Fact]
    public void SpendBonusPoints_WithZeroAmount_BonusPointsRemainUnchanged()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(50);

        // Act
        tourist.SpendBonusPoints(0);

        // Assert
        tourist.BonusPoints.Should().Be(50);
    }

    #endregion

    #region Combined Add and Spend Tests

    [Fact]
    public void BonusPoints_AddAndSpend_MaintainsCorrectBalance()
    {
        // Arrange
        var tourist = CreateValidTourist();

        // Act
        tourist.AddBonusPoints(100);
        tourist.SpendBonusPoints(30);
        tourist.AddBonusPoints(20);
        tourist.SpendBonusPoints(50);

        // Assert
        tourist.BonusPoints.Should().Be(40); // 100 - 30 + 20 - 50 = 40
    }

    [Fact]
    public void BonusPoints_NeverBecomesNegative()
    {
        // Arrange
        var tourist = CreateValidTourist();
        tourist.AddBonusPoints(50);

        // Act & Assert - First spend exact amount
        tourist.SpendBonusPoints(50);
        tourist.BonusPoints.Should().Be(0);

        // Act & Assert - Then try to spend more
        var act = () => tourist.SpendBonusPoints(1);
        act.Should().Throw<InvalidOperationException>();
        
        // Verify balance is still 0
        tourist.BonusPoints.Should().Be(0);
    }

    [Fact]
    public void NewTourist_StartsWithZeroBonusPoints()
    {
        // Arrange & Act
        var tourist = CreateValidTourist();

        // Assert
        tourist.BonusPoints.Should().Be(0);
    }

    #endregion
}
