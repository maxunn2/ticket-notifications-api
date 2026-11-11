using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Storage;

public interface ITicketRepository
{
    void AddTicket(Ticket ticket);
    Ticket? GetTicket(Guid id);
    void AddNotification(Notification notification);
    List<Notification> GetNotifications(Guid ticketId);
}