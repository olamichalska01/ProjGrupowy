using ComUnity.Application.Common.Models;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents;
using ComUnity.Application.Features.Notifications.Entities;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Notifications.EventHandlers;

internal class EventCreatedEventHandler : INotificationHandler<DomainEventNotification<EventCreatedEvent>>
{
    private readonly ComUnityContext _context;

    public EventCreatedEventHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<EventCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var userFriends = await _context.Set<Relationship>().Where(x => x.User1Id == domainEvent.CreatorId && x.RelationshipType == RelationshipTypes.Friendship).ToListAsync();

        var message = "{CreatorName} has created new event.";

        var notifications = userFriends.Select(friend => new  Notification(
            id: NewId.NextGuid(),
            userId: friend.User2Id,
            type: NotificationTypes.FriendHasAddedNewEvent,
            content: message,
            notificationDate: DateTime.UtcNow,
            expirationDate: null,
            additionalData: new Dictionary<string, string>() { { "CreatorName", domainEvent.CreatorName }, { "EventId", domainEvent.EventId.ToString() } })).ToList();

        _context.AddRange(notifications);
        await _context.SaveChangesAsync(cancellationToken);
    }
}