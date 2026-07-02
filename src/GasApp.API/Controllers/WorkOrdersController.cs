using GasApp.Application.Common.Interfaces;
using GasApp.Application.WorkOrders.Commands.AssignTechnician;
using GasApp.Application.WorkOrders.Commands.CreateWorkOrder;
using GasApp.Application.WorkOrders.Commands.StartWorkOrder;
using GasApp.Application.WorkOrders.Queries.GetTechnicianAgenda;
using GasApp.Application.WorkOrders.Queries.GetWorkOrderById;
using GasApp.Application.WorkOrders.Queries.GetWorkOrders;
using GasApp.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/work-orders")]
[Authorize]
public class WorkOrdersController(IMediator mediator, ICurrentUserService currentUser) : ControllerBase
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

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetWorkOrderByIdQuery(id), ct);
        return Ok(result);
    }

    [HttpGet("agenda")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> GetAgenda(
        [FromQuery] Guid? technicianId = null,
        [FromQuery] DateTime? date = null,
        CancellationToken ct = default)
    {
        var resolvedTechnicianId = currentUser.Role == UserRole.Technician
            ? currentUser.UserId
            : (technicianId ?? currentUser.UserId);

        var result = await mediator.Send(
            new GetTechnicianAgendaQuery(resolvedTechnicianId, date ?? DateTime.UtcNow.Date), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create([FromBody] CreateWorkOrderCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    [HttpPost("{id:guid}/assign")]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> AssignTechnician(Guid id, [FromBody] AssignTechnicianRequest request, CancellationToken ct)
    {
        await mediator.Send(new AssignTechnicianCommand(id, request.TechnicianId), ct);
        return NoContent();
    }

    [HttpPost("{id:guid}/start")]
    [Authorize(Roles = "Admin,Supervisor,Technician")]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        await mediator.Send(new StartWorkOrderCommand(id), ct);
        return NoContent();
    }
}

public record AssignTechnicianRequest(Guid TechnicianId);
