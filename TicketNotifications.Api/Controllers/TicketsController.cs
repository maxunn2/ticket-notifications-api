using Microsoft.AspNetCore.Mvc;
using TicketNotifications.Api.Models;
using TicketNotifications.Api.Services;

namespace TicketNotifications.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _service;

    public TicketsController(ITicketService service)
    {
        _service = service;
    }

    [HttpPost]
    public IActionResult Create(CreateTicketRequest request)
    {
        try
        {
            var ticket = _service.CreateTicket(request.Title, request.Description, request.Priority);
            var result = _service.GetTicketWithNotifications(ticket.Id)!.Value;
            return CreatedAtAction(nameof(Get), new { id = ticket.Id },
                new { result.Ticket, result.Notifications });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id}/notify")]
    public async Task<IActionResult> Notify(Guid id)
    {
        var found = await _service.NotifyAsync(id);
        if (!found)
        {
            return NotFound();
        }

        var result = _service.GetTicketWithNotifications(id)!.Value;
        return Ok(new { result.Ticket, result.Notifications });
    }

    [HttpGet("{id}")]
    public IActionResult Get(Guid id)
    {
        var result = _service.GetTicketWithNotifications(id);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(new { result.Value.Ticket, result.Value.Notifications });
    }
}