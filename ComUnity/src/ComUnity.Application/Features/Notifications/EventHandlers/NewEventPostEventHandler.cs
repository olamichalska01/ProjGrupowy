using ComUnity.Application.Common.Models;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.Notifications.Entities;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Notifications.EventHandlers;

internal class NewEventPostEventHandler : INotificationHandler<DomainEventNotification<NewEventPostEvent>>
{
    private readonly ComUnityContext _context;

    public NewEventPostEventHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<NewEventPostEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var eventt = await _context.Set<Event>().Include(x => x.Participants).FirstOrDefaultAsync(x => x.Id == domainEvent.EventId, cancellationToken);

        var message = "{CreatorName} has added new post in event {EventName}.";

        var notifications = eventt.Participants.Where(x => x.Username != domainEvent.CreatorName).Select(participant => new Notification(
            id: NewId.NextGuid(),
            userId: participant.UserId,
            type: NotificationTypes.NewEventPostAdded,
            content: message,
            notificationDate: DateTime.UtcNow,
            expirationDate: null,
            additionalData: new Dictionary<string, string>() { { "CreatorName", domainEvent.CreatorName }, { "EventId", domainEvent.EventId.ToString() }, { "EventName", eventt.EventName } })).ToList();

        _context.AddRange(notifications);
        await _context.SaveChangesAsync(cancellationToken);
    }
}