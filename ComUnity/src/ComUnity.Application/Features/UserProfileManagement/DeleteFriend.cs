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

public class DeleteFriendController : ApiControllerBase
{
    [HttpDelete("/api/users/{userId}/delete-friend")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteFriend([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteFriendCommand(userId), cancellationToken);

        return NoContent();
    }
}

public record DeleteFriendCommand(Guid UserId) : IRequest<Unit>;

internal class DeleteFriendCommandHandler : IRequestHandler<DeleteFriendCommand, Unit>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public DeleteFriendCommandHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<Unit> Handle(DeleteFriendCommand request, CancellationToken cancellationToken)
    {
        var sender = await _authenticatedUserProvider.GetUserProfile(cancellationToken);
        var receiverExists = await _context.Set<UserProfile>().AnyAsync(x => x.UserId == request.UserId, cancellationToken);

        if (!receiverExists)
        {
            throw new UserDoesNotExistException(request.UserId);
        }


        var relationship1 = await _context.Set<Relationship>().FirstOrDefaultAsync(x =>
            (x.User2Id == request.UserId && x.User1Id == sender.UserId),
            cancellationToken);
        var relationship2 = await _context.Set<Relationship>().FirstOrDefaultAsync(x =>
            (x.User1Id == request.UserId && x.User2Id == sender.UserId),
            cancellationToken);

        if ((relationship1 is null) || (relationship2 is null))
        {

            throw new FriendshipDoesNotExistsException();
        }

        _context.Remove(relationship1);
        _context.Remove(relationship2);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

public class DeleteFriendEvent : DomainEvent
{
    public string SenderName { get; }

    public Guid RequestId { get; }

    public Guid ReceiverId { get; }

    public DeleteFriendEvent(string senderName, Guid requestId, Guid receiverId)
    {
        SenderName = senderName;
        RequestId = requestId;
        ReceiverId = receiverId;
    }
}
