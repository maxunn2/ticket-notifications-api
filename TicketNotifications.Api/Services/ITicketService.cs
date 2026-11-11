using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Services;

public interface ITicketService
{
    Ticket CreateTicket(string title, string? description, Priority priority);
    Task<bool> NotifyAsync(Guid ticketId);
    (Ticket Ticket, List<Notification> Notifications)? GetTicketWithNotifications(Guid ticketId);
}