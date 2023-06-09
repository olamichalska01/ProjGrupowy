﻿using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Dtos;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq;
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
    ICollection<UserDto> UserFriends,
    ICollection<FavouriteCategory> FavouriteCategories,
    ICollection<UserEventsDto> UserEvents)
{
    public record FavouriteCategory(Guid Id, string Name, string? CategoyPic);
    public record UserEventsDto(
        Guid Id,
        string OwnerName,
        string Name,
        DateTime EventDate,
        double Cost,
        string EventCat,
        string? EventCategoryPicture
        );
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
            .Select(x => new FavouriteCategory(x.Id, x.CategoryName, x.ImageId.HasValue ? _azureStorageService.GetReadFileToken(x.ImageId.Value) : null))
            .ToListAsync(cancellationToken);

        var userFriendsRelations = await _context.Set<Relationship>()
            .Where(r => r.RelationshipType == "Friendship" && r.User2Id == user.UserId)
            .Select(x => x.User1Id)
            .ToListAsync();

        var userFriends = await _context
            .Set<UserProfile>()
            .Include(ur => ur.Relationships.Where(r => r.RelationshipType == "Friendship" && r.User2Id == user.UserId))
            .Where(u => u.UserId != user.UserId)
            .Where(r => userFriendsRelations.Contains(r.UserId))
            .Select(x => new UserDto(x.UserId, x.Username, x.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(x.ProfilePicture.Value) : null))
            .ToListAsync(cancellationToken);

        var userEvents = await _context
            .Set<Event>()
            .Where(e => e.Participants.Contains(user))
            .Select(x => new UserEventsDto (
                x.Id,
                x.Participants.FirstOrDefault(y => y.UserId == x.OwnerId).Username,
                x.EventName,
                x.StartDate,
                x.Cost, 
                x.EventCategory.CategoryName,
                x.EventCategory.ImageId.HasValue ? _azureStorageService.GetReadFileToken(x.EventCategory.ImageId.Value) : null))
            .ToListAsync();

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
            UserFriends: userFriends,
            UserEvents: userEvents);
    }

    private int CalculateAge(DateTime dateOfBirth)
    {
        var zero = new DateTime(1, 1, 1);
        var delta = DateTime.UtcNow - dateOfBirth;

        return (zero + delta).Year - 1;
    }
}
