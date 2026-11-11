using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Channels;

public class SmsChannel : INotificationChannel
{
    public Channel Channel => Channel.Sms;

    public Task SendAsync(Ticket ticket)
{
    if (ticket.Priority == Priority.Low)
    {
        throw new InvalidOperationException("SMS не надсилається для тікетів з низьким пріоритетом");
    }

    return Task.CompletedTask;
}
}