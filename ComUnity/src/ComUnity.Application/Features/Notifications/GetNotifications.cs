using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.Notifications.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.Notifications;

public class GetNotificationsController : ApiControllerBase
{
    [HttpGet("/api/notifications")]
    [ProducesResponseType(typeof(GetNotificationsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetNotificationsResponse>> GetNotifications(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetNotificationsQuery());
    }
}

public record GetNotificationsQuery : IRequest<GetNotificationsResponse>;

public record GetNotificationsResponse(ICollection<GetNotificationsResponse.Notification> Notifications)
{
    public record Notification(Guid Id, string Type, string Content, DateTime NotificationDate);
}


internal record GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, GetNotificationsResponse>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public GetNotificationsQueryHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<GetNotificationsResponse> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserProvider.GetUserId();

        var notifications = await _context.Set<Notification>().Where(x => x.UserId == userId).ToListAsync(cancellationToken);

        return new GetNotificationsResponse(notifications.Select(x => 
            new GetNotificationsResponse.Notification(
                x.Id,
                x.Type,
                x.Content,
                x.NotificationDate)).ToList());
    }
}