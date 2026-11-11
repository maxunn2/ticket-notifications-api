using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Channels;

public interface INotificationChannel
{
    Channel Channel { get; }
    Task SendAsync(Ticket ticket);
}