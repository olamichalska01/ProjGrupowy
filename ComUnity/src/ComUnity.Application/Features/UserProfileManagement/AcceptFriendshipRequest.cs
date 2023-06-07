using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Features.UserProfileManagement.Exceptions;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.UserProfileManagement;

public class AcceptFriendshipRequestController : ApiControllerBase
{
    [HttpPut("/api/friendship-requests/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AcceptFriendshipRequest([FromBody] AcceptFriendshipRequestCommand command, CancellationToken cancellationToken)
    {
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public record AcceptFriendshipRequestCommand(Guid UserId) : IRequest<Unit>;

internal class AcceptFriendshipRequestCommandHandler : IRequestHandler<AcceptFriendshipRequestCommand, Unit>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public AcceptFriendshipRequestCommandHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<Unit> Handle(AcceptFriendshipRequestCommand request, CancellationToken cancellationToken)
    {
        var user = await _authenticatedUserProvider.GetUserProfile(cancellationToken);

        var relationship = await _context.Set<Relationship>().Include(x => x.User2).FirstOrDefaultAsync(x => x.User1Id == request.UserId && x.User2Id == user.UserId, cancellationToken);

        if (relationship is null || relationship.RelationshipType != RelationshipTypes.FrienshipRequested)
        {
            throw new FriendshipRequestDoesNotExistException();
        }

        if(relationship.User2Id != user.UserId)
        {
            throw new FriendshipRequestDoesNotExistException();
        }

        relationship.AcceptFriendship();
        relationship.DomainEvents.Add(new FriendshipRequestAccepted(relationship.User1Id, relationship.User2Id, user.Username));
        var opositeDirectionRelationship = new Relationship(relationship.Id, relationship.User2Id, relationship.User1Id, RelationshipTypes.Friendship);
        _context.Add(opositeDirectionRelationship);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class FriendshipRequestAccepted : DomainEvent
{
    public Guid SenderId { get; }

    public Guid ReceiverId { get; }

    public string ReceiverName { get; }

    public FriendshipRequestAccepted(Guid senderId, Guid receiverId, string receiverName)
    {
        SenderId = senderId;
        ReceiverId = receiverId;
        ReceiverName = receiverName;
    }
}