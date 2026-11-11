using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Channels;

public class EmailChannel : INotificationChannel
{
    public Channel Channel => Channel.Email;

    public Task SendAsync(Ticket ticket)
    {
        if (string.IsNullOrWhiteSpace(ticket.Title))
        {
            throw new InvalidOperationException("Email не можна надіслати: порожній заголовок тікета");
        }

        return Task.CompletedTask;
    }
}