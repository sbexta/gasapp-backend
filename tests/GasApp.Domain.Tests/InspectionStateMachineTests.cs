using FluentAssertions;
using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;
using GasApp.Domain.Services;

namespace GasApp.Domain.Tests;

public class InspectionStateMachineTests
{
    [Theory]
    [InlineData(InspectionStatus.Pending, InspectionStatus.PreCheck, UserRole.Technician)]
    [InlineData(InspectionStatus.PreCheck, InspectionStatus.InProgress, UserRole.Supervisor)]
    [InlineData(InspectionStatus.InProgress, InspectionStatus.TechnicalReview, UserRole.Technician)]
    [InlineData(InspectionStatus.TechnicalReview, InspectionStatus.GeneratingDocs, UserRole.Supervisor)]
    [InlineData(InspectionStatus.TechnicalReview, InspectionStatus.Rejected, UserRole.Admin)]
    public void ValidTransition_ShouldNotThrow(InspectionStatus from, InspectionStatus to, UserRole role)
    {
        var act = () => InspectionStateMachine.ValidateTransition(from, to, role);
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(InspectionStatus.Pending, InspectionStatus.Completed, UserRole.Admin)]
    [InlineData(InspectionStatus.Completed, InspectionStatus.InProgress, UserRole.Admin)]
    [InlineData(InspectionStatus.Rejected, InspectionStatus.InProgress, UserRole.Admin)]
    public void InvalidTransition_ShouldThrowInvalidStateTransitionException(
        InspectionStatus from, InspectionStatus to, UserRole role)
    {
        var act = () => InspectionStateMachine.ValidateTransition(from, to, role);
        act.Should().Throw<InvalidStateTransitionException>();
    }

    [Fact]
    public void TechnicianCannotApproveInspection_ShouldThrowDomainException()
    {
        var act = () => InspectionStateMachine.ValidateTransition(
            InspectionStatus.TechnicalReview, InspectionStatus.GeneratingDocs, UserRole.Technician);

        act.Should().Throw<DomainException>()
            .WithMessage("*no está autorizado*");
    }

    [Fact]
    public void GetAllowedTransitions_FromPending_ReturnPreCheck()
    {
        var transitions = InspectionStateMachine.GetAllowedTransitions(InspectionStatus.Pending);
        transitions.Should().ContainSingle().Which.Should().Be(InspectionStatus.PreCheck);
    }

    [Fact]
    public void GetAllowedTransitions_FromCompleted_ReturnEmpty()
    {
        var transitions = InspectionStateMachine.GetAllowedTransitions(InspectionStatus.Completed);
        transitions.Should().BeEmpty();
    }
}
