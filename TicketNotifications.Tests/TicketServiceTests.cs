using TicketNotifications.Api.Channels;
using TicketNotifications.Api.Models;
using TicketNotifications.Api.Services;
using TicketNotifications.Api.Storage;

namespace TicketNotifications.Tests;

public class TicketServiceTests
{
    private static (TicketService Service, InMemoryTicketRepository Repo) CreateService(
        params INotificationChannel[] channels)
    {
        var repo = new InMemoryTicketRepository();
        var service = new TicketService(repo, channels);
        return (service, repo);
    }

    private static INotificationChannel[] SuccessfulChannels() => new INotificationChannel[]
    {
        new FakeChannel(Channel.Email),
        new FakeChannel(Channel.Sms),
        new FakeChannel(Channel.Push)
    };

    [Fact]
    public void CreateTicket_CreatesThreePendingNotifications()
    {
        var (service, repo) = CreateService(SuccessfulChannels());

        var ticket = service.CreateTicket("Valid title", null, Priority.High);

        var notifications = repo.GetNotifications(ticket.Id);
        Assert.Equal(3, notifications.Count);
        Assert.All(notifications, n => Assert.Equal(Status.Pending, n.Status));
    }
    private static INotificationChannel[] FailingSmsChannels(string error) => new INotificationChannel[]
    {
        new FakeChannel(Channel.Email),
        new FakeChannel(Channel.Sms, error),
        new FakeChannel(Channel.Push)
    };

    [Fact]
    public async Task Notify_SendsAllAndMarksSent()
    {
        var (service, repo) = CreateService(SuccessfulChannels());
        var ticket = service.CreateTicket("Valid title", null, Priority.High);

        await service.NotifyAsync(ticket.Id);

        var notifications = repo.GetNotifications(ticket.Id);
        Assert.All(notifications, n => Assert.Equal(Status.Sent, n.Status));
    }

    [Fact]
    public async Task Notify_OnFailure_IncrementsAttemptsAndStoresError()
    {
        var (service, repo) = CreateService(FailingSmsChannels("boom"));
        var ticket = service.CreateTicket("Valid title", null, Priority.High);

        await service.NotifyAsync(ticket.Id);

        var sms = repo.GetNotifications(ticket.Id).Single(n => n.Channel == Channel.Sms);
        Assert.Equal(Status.Failed, sms.Status);
        Assert.Equal(1, sms.Attempts);
        Assert.Equal("boom", sms.LastError);
    }

    [Fact]
    public async Task Notify_StopsRetryingAfterThreeAttempts()
    {
        var (service, repo) = CreateService(FailingSmsChannels("boom"));
        var ticket = service.CreateTicket("Valid title", null, Priority.High);

        for (int i = 0; i < 5; i++)
        {
            await service.NotifyAsync(ticket.Id);
        }

        var sms = repo.GetNotifications(ticket.Id).Single(n => n.Channel == Channel.Sms);
        Assert.Equal(3, sms.Attempts);
    }

    [Fact]
    public async Task Notify_IsIdempotent()
    {
        var (service, repo) = CreateService(SuccessfulChannels());
        var ticket = service.CreateTicket("Valid title", null, Priority.High);

        await service.NotifyAsync(ticket.Id);
        await service.NotifyAsync(ticket.Id);
        await service.NotifyAsync(ticket.Id);

        Assert.Equal(3, repo.GetNotifications(ticket.Id).Count);
    }
    [Fact]
    public void CreateTicket_WithShortTitle_Throws()
    {
        var (service, _) = CreateService(SuccessfulChannels());

        Assert.Throws<ArgumentException>(() => service.CreateTicket("abc", null, Priority.High));
    }



}