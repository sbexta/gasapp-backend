using MediatR;

namespace GasApp.Application.Inspections.Commands.SubmitInspection;

public record SubmitInspectionCommand(Guid InspectionId, string? TechnicianNotes) : IRequest;
