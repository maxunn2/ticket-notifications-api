using TicketNotifications.Api.Channels;
using TicketNotifications.Api.Models;

namespace TicketNotifications.Tests;

public class FakeChannel : INotificationChannel
{
    private readonly string? _errorMessage;

    public FakeChannel(Channel channel, string? errorMessage = null)
    {
        Channel = channel;
        _errorMessage = errorMessage;
    }

    public Channel Channel { get; }

    public Task SendAsync(Ticket ticket)
    {
        if (_errorMessage != null)
        {
            throw new InvalidOperationException(_errorMessage);
        }

        return Task.CompletedTask;
    }
}