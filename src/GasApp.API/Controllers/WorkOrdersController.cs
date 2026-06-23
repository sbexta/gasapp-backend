using GasApp.Application.WorkOrders.Commands.AssignTechnician;
using GasApp.Application.WorkOrders.Commands.CreateWorkOrder;
using GasApp.Application.WorkOrders.Queries.GetTechnicianAgenda;
using GasApp.Application.WorkOrders.Queries.GetWorkOrders;
using GasApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/work-orders")]
[Authorize]
public class WorkOrdersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] WorkOrderStatus? status = null,
        [FromQuery] Guid? technicianId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetWorkOrdersQuery(page, pageSize, status, technicianId, from, to), ct);
        return Ok(result);
    }

    [HttpGet("agenda")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> GetAgenda(
        [FromQuery] Guid technicianId,
        [FromQuery] DateTime? date = null,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(
            new GetTechnicianAgendaQuery(technicianId, date ?? DateTime.UtcNow.Date), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }

    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> AssignTechnician(Guid id, [FromBody] AssignTechnicianRequest request, CancellationToken ct)
    {
        await mediator.Send(new AssignTechnicianCommand(id, request.TechnicianId), ct);
        return NoContent();
    }
}

public record AssignTechnicianRequest(Guid TechnicianId);
