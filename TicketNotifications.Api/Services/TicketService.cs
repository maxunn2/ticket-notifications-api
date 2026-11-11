using TicketNotifications.Api.Channels;
using TicketNotifications.Api.Models;
using TicketNotifications.Api.Storage;

namespace TicketNotifications.Api.Services;

public class TicketService : ITicketService
{
    private const int MaxAttempts = 3;

    private readonly ITicketRepository _repository;
    private readonly IEnumerable<INotificationChannel> _channels;

    public TicketService(ITicketRepository repository, IEnumerable<INotificationChannel> channels)
    {
        _repository = repository;
        _channels = channels;
    }

    public Ticket CreateTicket(string title, string? description, Priority priority)
    {
        if (string.IsNullOrWhiteSpace(title) || title.Trim().Length < 5)
        {
            throw new ArgumentException("Title обов'язковий і має містити мінімум 5 символів");
        }

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description,
            Priority = priority,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _repository.AddTicket(ticket);

        foreach (var channel in Enum.GetValues<Channel>())
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                TicketId = ticket.Id,
                Channel = channel,
                Status = Status.Pending,
                Attempts = 0,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _repository.AddNotification(notification);
        }

        return ticket;
    }

        public (Ticket Ticket, List<Notification> Notifications)? GetTicketWithNotifications(Guid ticketId)
    {
        var ticket = _repository.GetTicket(ticketId);
        if (ticket == null)
        {
            return null;
        }

        return (ticket, _repository.GetNotifications(ticketId));
    }



        public async Task<bool> NotifyAsync(Guid ticketId)
    {
        var ticket = _repository.GetTicket(ticketId);
        if (ticket == null)
        {
            return false;
        }

        var notifications = _repository.GetNotifications(ticketId);

        foreach (var notification in notifications)
        {
            if (notification.Status == Status.Sent || notification.Attempts >= MaxAttempts)
            {
                continue;
            }

            var channel = _channels.First(c => c.Channel == notification.Channel);

            try
            {
                await channel.SendAsync(ticket);
                notification.Status = Status.Sent;
                notification.LastError = null;
            }
            catch (Exception ex)
            {
                notification.Attempts++;
                notification.Status = Status.Failed;
                notification.LastError = ex.Message;
            }
        }

        return true;
    }


}
