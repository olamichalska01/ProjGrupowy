using ComUnity.Application.Common.Models;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Notifications.Entities;
using ComUnity.Application.Features.UserProfileManagement;
using MassTransit;
using MediatR;

namespace ComUnity.Application.Features.Notifications.EventHandlers
{
    internal class FriendshipRequestAcceptedEventHandler : INotificationHandler<DomainEventNotification<FriendshipRequestAccepted>>
    {
        private readonly ComUnityContext _context;

        public FriendshipRequestAcceptedEventHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task Handle(DomainEventNotification<FriendshipRequestAccepted> notification, CancellationToken cancellationToken)
        {
            var domainEvent = notification.DomainEvent;
            var message = $"{domainEvent.ReceiverName} has accepted your friend request.";
            var newNotification = new Notification(
                id: NewId.NextGuid(),
                userId: domainEvent.SenderId,
                type: NotificationTypes.FriendshipRequestAccepted,
                content: message,
                notificationDate: DateTime.UtcNow,
                expirationDate: DateTime.UtcNow.AddDays(2),
                additionalData: null);

            _context.Add(newNotification);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
