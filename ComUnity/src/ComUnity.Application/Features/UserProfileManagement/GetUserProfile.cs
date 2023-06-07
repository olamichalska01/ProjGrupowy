using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ComUnity.Application.Features.UserProfileManagement.GetUserProfileResponse;

namespace ComUnity.Application.Features.UserProfileManagement;

public class GetUserProfileController : ApiControllerBase
{
    [HttpGet("/api/users/{userId}")]
    [ProducesResponseType(typeof(GetUserProfileResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetUserProfileResponse>> GetUserProfile([FromRoute] Guid userId, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetUserProfileQuery(userId));
    }
}

public record GetUserProfileQuery(Guid UserId) : IRequest<GetUserProfileResponse>;

public record GetUserProfileResponse(
    Guid UserId,
    string Username,
    int? Age,
    string? AboutMe,
    string? City,
    string? ProfilePicture,
    bool IsFriend,
    bool IsFriendshipRequestSent,
    bool IsFriendshipRequestReceived,
    ICollection<FavouriteCategory> FavouriteCategories,
    ICollection<ParticipatedInEvent> ParticipatedInEvents)
{
    public record FavouriteCategory(Guid Id, string Name);
    public record ParticipatedInEvent(Guid Id, string Name, string Category);
}

internal class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, GetUserProfileResponse>
{
    private readonly ComUnityContext _context;
    private readonly IAzureStorageService _azureStorageService;
    private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

    public GetUserProfileQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService, IAuthenticatedUserProvider authenticatedUserProvider)
    {
        _context = context;
        _azureStorageService = azureStorageService;
        _authenticatedUserProvider = authenticatedUserProvider;
    }

    public async Task<GetUserProfileResponse> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        var requestUserId = _authenticatedUserProvider.GetUserId();

        var user = await _context
            .Set<UserProfile>()
            .Include(x => x.FavoriteCategories)
            .Include(x => x.Relationships.Where(r => r.User2Id == requestUserId))
            .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        var favouriteCategories = await _context
            .Set<EventCategory>()
            .Where(x => user.FavoriteCategories.Select(x => x.EventCategoryId).Contains(x.Id))
            .Select(x => new FavouriteCategory(x.Id, x.CategoryName))
            .ToListAsync(cancellationToken);

        var profilePicture = user.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(user.ProfilePicture.Value) : null;
        var requestUserSideRelationship = await _context.Set<Relationship>().FirstOrDefaultAsync(x => x.User1Id == requestUserId && x.User2Id == user.UserId, cancellationToken);
        var fetchedUserSideRelationship = user.Relationships?.FirstOrDefault(x => x.User2Id == requestUserId);

        return new GetUserProfileResponse(
            UserId: user.UserId,
            Username: user.Username,
            AboutMe: user.AboutMe,
            Age: CalculateAge(user.DateOfBirth),
            City: user.City,
            ProfilePicture: profilePicture,
            IsFriendshipRequestSent: requestUserSideRelationship?.RelationshipType == RelationshipTypes.FrienshipRequested,
            IsFriendshipRequestReceived: fetchedUserSideRelationship?.RelationshipType == RelationshipTypes.FrienshipRequested,
            IsFriend: requestUserSideRelationship?.RelationshipType == RelationshipTypes.Friendship,
            FavouriteCategories: favouriteCategories,
            ParticipatedInEvents: Array.Empty<ParticipatedInEvent>());
    }

    private int CalculateAge(DateTime dateOfBirth)
    {
        var zero = new DateTime(1, 1, 1);
        var delta = DateTime.UtcNow - dateOfBirth;

        return (zero + delta).Year - 1;
    }
}
