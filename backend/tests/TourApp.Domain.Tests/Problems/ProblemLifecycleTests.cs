using FluentAssertions;
using TourApp.Domain.Problems;

namespace TourApp.Domain.Tests.Problems;

public class ProblemLifecycleTests
{
    private static Problem CreateValidProblem()
    {
        return new Problem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Test Problem Title",
            "Test Problem Description");
    }

    #region Initial State Tests

    [Fact]
    public void NewProblem_HasPendingStatus()
    {
        // Act
        var problem = CreateValidProblem();

        // Assert
        problem.Status.Should().Be(ProblemStatus.Pending);
    }

    [Fact]
    public void NewProblem_HasNoEvents()
    {
        // Act
        var problem = CreateValidProblem();

        // Assert
        problem.Events.Should().BeEmpty();
    }

    [Fact]
    public void NewProblem_HasCreatedAtSet()
    {
        // Act
        var beforeCreate = DateTime.UtcNow;
        var problem = CreateValidProblem();
        var afterCreate = DateTime.UtcNow;

        // Assert
        problem.CreatedAt.Should().BeOnOrAfter(beforeCreate);
        problem.CreatedAt.Should().BeOnOrBefore(afterCreate);
    }

    #endregion

    #region Resolve Tests

    [Fact]
    public void Resolve_FromPending_ChangesStatusToResolved()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        problem.Resolve(guideId);

        // Assert
        problem.Status.Should().Be(ProblemStatus.Resolved);
    }

    [Fact]
    public void Resolve_FromPending_CreatesExactlyOneEvent()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        problem.Resolve(guideId);

        // Assert
        problem.Events.Should().HaveCount(1);
    }

    [Fact]
    public void Resolve_FromPending_EventContainsCorrectData()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        var statusEvent = problem.Resolve(guideId);

        // Assert
        statusEvent.ProblemId.Should().Be(problem.Id);
        statusEvent.OldStatus.Should().Be(ProblemStatus.Pending);
        statusEvent.NewStatus.Should().Be(ProblemStatus.Resolved);
        statusEvent.ChangedAt.Should().NotBe(default);
        statusEvent.ChangedByRole.Should().Be("Guide");
        statusEvent.ChangedByUserId.Should().Be(guideId);
    }

    [Fact]
    public void Resolve_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        problem.SendToReview(guideId); // Change to InReview

        // Act
        var act = () => problem.Resolve(guideId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only pending problems can be resolved*");
    }

    #endregion

    #region SendToReview Tests

    [Fact]
    public void SendToReview_FromPending_ChangesStatusToInReview()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        problem.SendToReview(guideId);

        // Assert
        problem.Status.Should().Be(ProblemStatus.InReview);
    }

    [Fact]
    public void SendToReview_FromPending_CreatesExactlyOneEvent()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        problem.SendToReview(guideId);

        // Assert
        problem.Events.Should().HaveCount(1);
    }

    [Fact]
    public void SendToReview_FromPending_EventContainsCorrectData()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        var statusEvent = problem.SendToReview(guideId);

        // Assert
        statusEvent.ProblemId.Should().Be(problem.Id);
        statusEvent.OldStatus.Should().Be(ProblemStatus.Pending);
        statusEvent.NewStatus.Should().Be(ProblemStatus.InReview);
        statusEvent.ChangedAt.Should().NotBe(default);
        statusEvent.ChangedByRole.Should().Be("Guide");
        statusEvent.ChangedByUserId.Should().Be(guideId);
    }

    [Fact]
    public void SendToReview_WhenNotPending_ThrowsInvalidOperationException()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        problem.Resolve(guideId); // Change to Resolved

        // Act
        var act = () => problem.SendToReview(guideId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only pending problems can be sent for review*");
    }

    #endregion

    #region Reject Tests

    [Fact]
    public void Reject_FromInReview_ChangesStatusToRejected()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        problem.SendToReview(guideId);

        // Act
        problem.Reject(adminId);

        // Assert
        problem.Status.Should().Be(ProblemStatus.Rejected);
    }

    [Fact]
    public void Reject_FromInReview_CreatesEvent()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        problem.SendToReview(guideId);

        // Act
        problem.Reject(adminId);

        // Assert
        problem.Events.Should().HaveCount(2); // SendToReview + Reject
    }

    [Fact]
    public void Reject_FromInReview_EventContainsCorrectData()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        problem.SendToReview(guideId);

        // Act
        var statusEvent = problem.Reject(adminId);

        // Assert
        statusEvent.ProblemId.Should().Be(problem.Id);
        statusEvent.OldStatus.Should().Be(ProblemStatus.InReview);
        statusEvent.NewStatus.Should().Be(ProblemStatus.Rejected);
        statusEvent.ChangedAt.Should().NotBe(default);
        statusEvent.ChangedByRole.Should().Be("Admin");
        statusEvent.ChangedByUserId.Should().Be(adminId);
    }

    [Fact]
    public void Reject_WhenNotInReview_ThrowsInvalidOperationException()
    {
        // Arrange
        var problem = CreateValidProblem();
        var adminId = Guid.NewGuid();

        // Act
        var act = () => problem.Reject(adminId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only problems in review can be rejected*");
    }

    #endregion

    #region Reopen Tests

    [Fact]
    public void Reopen_FromInReview_ChangesStatusToPending()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        problem.SendToReview(guideId);

        // Act
        problem.Reopen(adminId);

        // Assert
        problem.Status.Should().Be(ProblemStatus.Pending);
    }

    [Fact]
    public void Reopen_FromInReview_CreatesEvent()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        problem.SendToReview(guideId);

        // Act
        problem.Reopen(adminId);

        // Assert
        problem.Events.Should().HaveCount(2); // SendToReview + Reopen
    }

    [Fact]
    public void Reopen_FromInReview_EventContainsCorrectData()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();
        problem.SendToReview(guideId);

        // Act
        var statusEvent = problem.Reopen(adminId);

        // Assert
        statusEvent.ProblemId.Should().Be(problem.Id);
        statusEvent.OldStatus.Should().Be(ProblemStatus.InReview);
        statusEvent.NewStatus.Should().Be(ProblemStatus.Pending);
        statusEvent.ChangedAt.Should().NotBe(default);
        statusEvent.ChangedByRole.Should().Be("Admin");
        statusEvent.ChangedByUserId.Should().Be(adminId);
    }

    [Fact]
    public void Reopen_WhenNotInReview_ThrowsInvalidOperationException()
    {
        // Arrange
        var problem = CreateValidProblem();
        var adminId = Guid.NewGuid();

        // Act
        var act = () => problem.Reopen(adminId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only problems in review can be reopened*");
    }

    #endregion

    #region Event Accumulation Tests

    [Fact]
    public void MultipleStatusChanges_AccumulateEvents()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        // Act
        problem.SendToReview(guideId);    // Pending -> InReview
        problem.Reopen(adminId);           // InReview -> Pending
        problem.Resolve(guideId);          // Pending -> Resolved

        // Assert
        problem.Events.Should().HaveCount(3);
        problem.Status.Should().Be(ProblemStatus.Resolved);
    }

    [Fact]
    public void Events_AreReturnedFromStatusChangeMethods()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();

        // Act
        var event1 = problem.SendToReview(guideId);

        // Assert
        event1.Should().NotBeNull();
        event1.Id.Should().NotBe(Guid.Empty);
        problem.Events.Should().Contain(event1);
    }

    #endregion

    #region ProblemStatusChangedEvent Validation Tests

    [Fact]
    public void Event_HasValidTimestamp()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var beforeChange = DateTime.UtcNow;

        // Act
        var statusEvent = problem.Resolve(guideId);
        var afterChange = DateTime.UtcNow;

        // Assert
        statusEvent.ChangedAt.Should().BeOnOrAfter(beforeChange);
        statusEvent.ChangedAt.Should().BeOnOrBefore(afterChange);
    }

    [Fact]
    public void Event_HasUniqueId()
    {
        // Arrange
        var problem = CreateValidProblem();
        var guideId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        // Act
        var event1 = problem.SendToReview(guideId);
        var event2 = problem.Reopen(adminId);

        // Assert
        event1.Id.Should().NotBe(event2.Id);
        event1.Id.Should().NotBe(Guid.Empty);
        event2.Id.Should().NotBe(Guid.Empty);
    }

    #endregion
}
