using GasApp.Application.ChecklistTemplates.Commands.AddChecklistItem;
using GasApp.Application.ChecklistTemplates.Commands.AddChecklistSection;
using GasApp.Application.ChecklistTemplates.Commands.CreateChecklistTemplate;
using GasApp.Application.ChecklistTemplates.Queries.GetChecklistTemplateDetail;
using GasApp.Application.ChecklistTemplates.Queries.GetChecklistTemplates;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/checklist-templates")]
[Authorize(Roles = "Admin,Supervisor")]
public class ChecklistTemplatesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await mediator.Send(new GetChecklistTemplatesQuery(), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetChecklistTemplateDetailQuery(id), ct);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTemplateRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateChecklistTemplateCommand(
            request.Name, request.Description, request.InspectionTypeId), ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/sections")]
    public async Task<IActionResult> AddSection(Guid id, [FromBody] AddSectionRequest request, CancellationToken ct)
    {
        var sectionId = await mediator.Send(new AddChecklistSectionCommand(id, request.Name, request.Order), ct);
        return Ok(new { id = sectionId });
    }

    [HttpPost("{id:guid}/sections/{sectionId:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, Guid sectionId, [FromBody] AddItemRequest request, CancellationToken ct)
    {
        var itemId = await mediator.Send(new AddChecklistItemCommand(
            sectionId, request.Question, request.ItemType, request.Order, request.IsRequired, request.HelpText), ct);
        return Ok(new { id = itemId });
    }
}

public class CreateTemplateRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public Guid? InspectionTypeId { get; set; }
}

public class AddSectionRequest
{
    public string Name { get; set; } = null!;
    public int Order { get; set; }
}

public class AddItemRequest
{
    public string Question { get; set; } = null!;
    public string ItemType { get; set; } = "YesNo";
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? HelpText { get; set; }
}
