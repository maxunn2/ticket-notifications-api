namespace TicketNotifications.Api.Models;

public class Notification
{
    public Guid Id {get; set;}
    public Guid TicketId {get; set;}
    public Channel Channel {get; set;}
    public Status Status {get; set;}
    public int Attempts {get; set;}
    public string? LastError {get; set;}

    public DateTimeOffset CreatedAt {get; set;}





}