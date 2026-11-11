using TicketNotifications.Api.Models;

namespace TicketNotifications.Api.Channels;

public class PushChannel : INotificationChannel
{
    public Channel Channel => Channel.Push;

      public Task SendAsync(Ticket ticket)
    {
        if (ticket.Title.Length > 50)
        {
            throw new InvalidOperationException("Push не можна надіслати: заголовок довший за 50 символів");
        }

        return Task.CompletedTask;
    }
}