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

public class GetFriendshipsController : ApiControllerBase
{
    [HttpGet("/api/firendships")]
    [ProducesResponseType(typeof(GetFriendshipsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetFriendshipsQueryResponse>> GetFriendships(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetFriendshipsQuery(), cancellationToken);
    }
}

public record GetFriendshipsQuery() : IRequest<GetFriendshipsQueryResponse>;

public record GetFriendshipsQueryResponse(ICollection<GetFriendshipsQueryResponse.Friendship> Friendships)
{
    public record Friendship(Guid FriendId);
}

internal class GetFriendshipsQueryHandler : IRequestHandler<GetFriendshipsQuery, GetFriendshipsQueryResponse>
{
    private readonly ComUnityContext _context;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public GetFriendshipsQueryHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<GetFriendshipsQueryResponse> Handle(GetFriendshipsQuery request, CancellationToken cancellationToken)
    {
        var userId = _authenticatedUserProvider.GetUserId();

        var userWithFriendships = await _context.Set<UserProfile>()
            .Include(x => x.Relationships != null ? x.Relationships.Where(r => r.RelationshipType == RelationshipTypes.Friendship) : null)
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

        return userWithFriendships is null
            ? throw new NotFoundException($"User with Id {userId} not found")
            : new GetFriendshipsQueryResponse(
            userWithFriendships.Relationships is null
            ? new List<GetFriendshipsQueryResponse.Friendship>()
            : userWithFriendships.Relationships.Select(x => new GetFriendshipsQueryResponse.Friendship(x.User2Id)).ToList());
    }
}