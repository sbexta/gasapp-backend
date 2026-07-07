using GasApp.Application.ChecklistTemplates.Commands.AddChecklistItem;
using GasApp.Application.ChecklistTemplates.Commands.AddChecklistSection;
using GasApp.Application.ChecklistTemplates.Commands.CreateChecklistTemplate;
using GasApp.Application.ChecklistTemplates.Commands.DeleteChecklistItem;
using GasApp.Application.ChecklistTemplates.Commands.DeleteChecklistSection;
using GasApp.Application.ChecklistTemplates.Commands.ToggleChecklistTemplate;
using GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistItem;
using GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistSection;
using GasApp.Application.ChecklistTemplates.Commands.UpdateChecklistTemplate;
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

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTemplateRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateChecklistTemplateCommand(id, request.Name, request.Description), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/toggle")]
    public async Task<IActionResult> Toggle(Guid id, CancellationToken ct)
    {
        await mediator.Send(new ToggleChecklistTemplateCommand(id), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/sections/{sectionId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, Guid sectionId, Guid itemId, [FromBody] UpdateItemRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateChecklistItemCommand(
            itemId, request.Question, request.ItemType, request.Order, request.IsRequired, request.HelpText), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/sections/{sectionId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, Guid sectionId, Guid itemId, CancellationToken ct)
    {
        await mediator.Send(new DeleteChecklistItemCommand(itemId), ct);
        return NoContent();
    }

    [HttpPut("{id:guid}/sections/{sectionId:guid}")]
    public async Task<IActionResult> UpdateSection(Guid id, Guid sectionId, [FromBody] UpdateSectionRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateChecklistSectionCommand(sectionId, request.Name, request.Order), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}/sections/{sectionId:guid}")]
    public async Task<IActionResult> DeleteSection(Guid id, Guid sectionId, CancellationToken ct)
    {
        await mediator.Send(new DeleteChecklistSectionCommand(sectionId), ct);
        return NoContent();
    }
}

public class UpdateTemplateRequest
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}

public class UpdateItemRequest
{
    public string Question { get; set; } = null!;
    public string ItemType { get; set; } = "YesNo";
    public int Order { get; set; }
    public bool IsRequired { get; set; } = true;
    public string? HelpText { get; set; }
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

public class UpdateSectionRequest
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
