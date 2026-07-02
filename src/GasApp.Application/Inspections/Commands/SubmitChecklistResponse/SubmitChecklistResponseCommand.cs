using MediatR;

namespace GasApp.Application.Inspections.Commands.SubmitChecklistResponse;

public record SubmitChecklistResponseCommand(
    Guid InspectionId,
    Guid ChecklistItemId,
    string? TextValue,
    bool? BoolValue,
    decimal? NumericValue,
    bool Complies,
    string? Notes
) : IRequest;
