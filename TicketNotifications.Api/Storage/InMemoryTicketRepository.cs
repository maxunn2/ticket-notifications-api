using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Storage;

public class InMemoryTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets = new();
    private readonly List<Notification> _notifications = new();

    public void AddTicket(Ticket ticket)
    {
        _tickets.Add(ticket);
    }

    public Ticket? GetTicket(Guid id)
    {
        return _tickets.FirstOrDefault(t => t.Id == id);
    }

    public void AddNotification(Notification notification)
    {
        _notifications.Add(notification);
    }

    public List<Notification> GetNotifications(Guid ticketId)
    {
        return _notifications.Where(n => n.TicketId == ticketId).ToList();
    }



}