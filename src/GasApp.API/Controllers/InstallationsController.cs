using GasApp.Application.Installations.Commands.CreateInstallation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/installations")]
[Authorize(Roles = "Admin,Supervisor,Technician")]
public class InstallationsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Admin,Supervisor")]
    public async Task<IActionResult> Create([FromBody] CreateInstallationCommand command, CancellationToken ct)
    {
        var id = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(Create), new { id }, new { id });
    }
}
