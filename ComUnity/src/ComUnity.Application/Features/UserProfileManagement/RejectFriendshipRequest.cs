using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.UserProfileManagement;

public class RejectFriendshipRequestController : ApiControllerBase
{
    [HttpDelete("/api/friendship-requests/{requestId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> RejectFriendshipRequest([FromRoute] Guid requestId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new RejectFriendshipRequestCommand(requestId), cancellationToken);

        return NoContent();
    }
}

public record RejectFriendshipRequestCommand(Guid RequestId) : IRequest<Unit>;

internal class RejectFriendshipRequestCommandHandler : IRequestHandler<RejectFriendshipRequestCommand, Unit>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public RejectFriendshipRequestCommandHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _authenticatedUserProvider = authenticatedUserProvider;
        _context = context;
    }

    public async Task<Unit> Handle(RejectFriendshipRequestCommand request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserProvider.GetUserId();
        var friendshipRequest = await _context.Set<Relationship>().FirstOrDefaultAsync(x => x.RelationshipType == RelationshipTypes.FrienshipRequested && (x.User2Id == userId || x.User1Id == userId), cancellationToken);

        if (friendshipRequest == null)
        {
            throw new NotFoundException("Friendship request not found");
        }

        _context.Remove(friendshipRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
