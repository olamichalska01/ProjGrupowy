using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Notifications.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Notifications;

public class DeleteNotificationController : ApiControllerBase
{
    [HttpDelete("/api/notifications/{notificationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteNotification(Guid notificationId)
    {
        await Mediator.Send(new DeleteNotificationCommand(notificationId));

        return NoContent();
    }
}

public record DeleteNotificationCommand(Guid NotificationId) : IRequest<Unit>;

internal class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, Unit>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public DeleteNotificationCommandHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<Unit> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserProvider.GetUserId();
        var notification = await _context.Set<Notification>().FirstOrDefaultAsync(x => x.Id == request.NotificationId && x.UserId == userId, cancellationToken);

        if (notification == null)
        {
            throw new NotFoundException();
        }

        _context.Remove(notification);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
