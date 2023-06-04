using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Features.UserProfileManagement.Exceptions;
using ComUnity.Application.Infrastructure.Services;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.UserProfileManagement;

public class AddFriendController : ApiControllerBase
{
    [HttpPut("/api/users/{userId}/add-friend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddFriend([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new AddFriendCommand(userId), cancellationToken);

        return NoContent();
    }
}

public record AddFriendCommand(Guid UserId) : IRequest<Unit>;

internal class AddFriendCommandHandler : IRequestHandler<AddFriendCommand, Unit>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public AddFriendCommandHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<Unit> Handle(AddFriendCommand request, CancellationToken cancellationToken)
    {
        var sender = await _authenticatedUserProvider.GetUserProfile(cancellationToken);
        var receiverExists = await _context.Set<UserProfile>().AnyAsync(x => x.UserId == request.UserId, cancellationToken);

        if(!receiverExists)
        {
            throw new UserDoesNotExistException(request.UserId);
        }


        var relationship = await _context.Set<Relationship>().FirstOrDefaultAsync(x => 
            (x.User1Id == request.UserId && x.User2Id == sender.UserId) || 
            (x.User2Id == request.UserId && x.User1Id == sender.UserId),
            cancellationToken);

        if(relationship is not null)
        {
            if(relationship.RelationshipType == RelationshipTypes.Blocked)
            {
                return Unit.Value;
            }

            throw new FriendshipArleadyExistsException();
        }

        var relationshipId = NewId.NextGuid();

        relationship = new Relationship(relationshipId, sender.UserId, request.UserId, RelationshipTypes.FrienshipRequested);
        relationship.DomainEvents.Add(new FriendRequestSentEvent(sender.Username, relationshipId, request.UserId));
        _context.Add(relationship);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class FriendRequestSentEvent : DomainEvent
{
    public string SenderName { get; }

    public Guid RequestId { get; }

    public Guid ReceiverId { get; }

    public FriendRequestSentEvent(string senderName, Guid requestId, Guid receiverId)
    {
        SenderName = senderName;
        RequestId = requestId;
        ReceiverId = receiverId;
    }
}
