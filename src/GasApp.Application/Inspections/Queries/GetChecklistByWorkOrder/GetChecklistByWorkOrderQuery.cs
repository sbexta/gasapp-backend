using MediatR;

namespace GasApp.Application.Inspections.Queries.GetChecklistByWorkOrder;

public record GetChecklistByWorkOrderQuery(Guid WorkOrderId) : IRequest<WorkOrderChecklistDto?>;

public record WorkOrderChecklistDto(
    Guid InspectionId,
    IReadOnlyList<ChecklistSectionDto> Sections
);

public record ChecklistSectionDto(
    Guid Id,
    string Name,
    int Order,
    IReadOnlyList<ChecklistItemDto> Items
);

public record ChecklistItemDto(
    Guid Id,
    string Question,
    string ItemType,
    bool IsRequired,
    int Order,
    string? HelpText,
    ChecklistItemResponseDto? Response
);

public record ChecklistItemResponseDto(
    Guid ResponseId,
    string? TextValue,
    bool? BoolValue,
    decimal? NumericValue,
    bool Complies,
    string? Notes
);
