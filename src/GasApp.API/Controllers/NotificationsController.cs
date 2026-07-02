using GasApp.Application.Notifications.Commands.MarkNotificationRead;
using GasApp.Application.Notifications.Queries.GetNotifications;
using GasApp.Application.Notifications.Queries.GetUnreadCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GasApp.API.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetNotificationsQuery(page, pageSize), ct);
        return Ok(result);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> UnreadCount(CancellationToken ct)
    {
        var count = await mediator.Send(new GetUnreadCountQuery(), ct);
        return Ok(new { count });
    }

    [HttpPut("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(Guid id, CancellationToken ct)
    {
        await mediator.Send(new MarkNotificationReadCommand(id), ct);
        return NoContent();
    }
}
