using ComUnity.Application.Common.Models;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Notifications.Entities;
using ComUnity.Application.Features.UserProfileManagement;
using MassTransit;
using MediatR;

namespace ComUnity.Application.Features.Notifications.EventHandlers;

internal class FriendRequestSentEventHandler : INotificationHandler<DomainEventNotification<FriendRequestSentEvent>>
{
    private readonly ComUnityContext _context;

    public FriendRequestSentEventHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<FriendRequestSentEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        var message = "{SenderName} has sent you a friend request.";
        var newNotification = new Notification(
            id: NewId.NextGuid(),
            userId: domainEvent.ReceiverId,
            type: NotificationTypes.FriendshipRequested,
            content: message,
            notificationDate: DateTime.UtcNow,
            expirationDate: null,
            additionalData: new Dictionary<string, string>() { { "SenderId", domainEvent.SenderId.ToString() }, { "SenderName", domainEvent.SenderName } });

        _context.Add(newNotification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
