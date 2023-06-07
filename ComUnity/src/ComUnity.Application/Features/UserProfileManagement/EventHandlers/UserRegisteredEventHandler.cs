using ComUnity.Application.Common.Models;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Authentication;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using MediatR;

namespace ComUnity.Application.Features.UserProfileManagement.EventHandlers;

internal class UserRegisteredEventHandler : INotificationHandler<DomainEventNotification<UserRegisteredEvent>>
{
    private readonly ComUnityContext _context;

    public UserRegisteredEventHandler(ComUnityContext context)
    {
        _context = context;
    }

    public async Task Handle(DomainEventNotification<UserRegisteredEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var userProfile = new UserProfile(domainEvent.UserId, domainEvent.Username, domainEvent.DateOfBirth);

        _context.Set<UserProfile>().Add(userProfile);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
