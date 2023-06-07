namespace ComUnity.Application.Features.Notifications.Entities;

public class Notification
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public string Type { get; private set; }

    public string Content { get; private set; }

    public DateTime NotificationDate { get; private set; }

    public DateTime ExpirationDate { get; private set; }

    public Dictionary<string, string>? AdditionalData { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Notification() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Notification(Guid id, Guid userId, string type, string content, DateTime notificationDate, DateTime? expirationDate, Dictionary<string, string>? additionalData)
    {
        Id = id;
        UserId = userId;
        Type = type;
        Content = content;
        NotificationDate = notificationDate;
        ExpirationDate = expirationDate ?? DateTime.MaxValue;
        AdditionalData = additionalData;
    }
}
