using MediatR;

namespace GasApp.Application.Inspections.Commands.ApproveInspection;

public record ApproveInspectionCommand(Guid InspectionId, string? SupervisorNotes) : IRequest;
