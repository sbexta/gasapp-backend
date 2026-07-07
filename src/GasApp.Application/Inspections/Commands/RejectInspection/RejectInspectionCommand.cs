using MediatR;

namespace GasApp.Application.Inspections.Commands.RejectInspection;

public record RejectInspectionCommand(Guid InspectionId, string? SupervisorNotes) : IRequest;
