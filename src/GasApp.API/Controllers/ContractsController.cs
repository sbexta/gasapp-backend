using GasApp.Application.Contracts.Commands.CreateContract;
using GasApp.Application.Contracts.Queries.GetContracts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/contracts")]
[Authorize(Roles = "Admin,Supervisor")]
public class ContractsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? clientId, CancellationToken ct)
    {
        var result = await mediator.Send(new GetContractsQuery(clientId), ct);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateContractCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetAll), new { id }, new { id });
    }
}
