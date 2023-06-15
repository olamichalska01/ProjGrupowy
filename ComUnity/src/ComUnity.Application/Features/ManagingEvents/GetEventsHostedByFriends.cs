using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Dtos;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComUnity.Application.Features.ManagingEvents
{
    public class GetEventsHostedByFriendsController : ApiControllerBase
    {
        [HttpGet("/api/friendsEvents")]
        [ProducesResponseType(typeof(GetFriendsEventsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<GetFriendsEventsResponse>> GetEvents([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            return await Mediator.Send(new GetFriendsEventsQuery(userId));
        }

        public record GetFriendsEventsQuery(Guid UserId) : IRequest<GetFriendsEventsResponse>;

        public record GetFriendsEventsResponse(ICollection<EventDto> Events);

        internal class GetEventByIdQueryHandler : IRequestHandler<GetFriendsEventsQuery, GetFriendsEventsResponse>
        {
            private readonly ComUnityContext _context;
            private readonly IAzureStorageService _azureStorageService;
            private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

            public GetEventByIdQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService, IAuthenticatedUserProvider authenticatedUserProvider)
            {
                _context = context;
                _azureStorageService = azureStorageService;
                _authenticatedUserProvider = authenticatedUserProvider;
            }

            public async Task<GetFriendsEventsResponse> Handle(GetFriendsEventsQuery request, CancellationToken cancellationToken)
            {
                var requestUserId = _authenticatedUserProvider.GetUserId();

                var userFriendsRelations = await _context.Set<Relationship>()
                    .Where(r => r.RelationshipType == "Friendship" && r.User2Id == requestUserId)
                    .Select(x => x.User1Id)
                    .ToListAsync(cancellationToken);
                
                var userFriendsIds = await _context
                    .Set<UserProfile>()
                    .Include(ur => ur.Relationships.Where(r => r.RelationshipType == "Friendship" && r.User2Id == requestUserId))
                    .Where(u => u.UserId != requestUserId)
                    .Where(r => userFriendsRelations.Contains(r.UserId))
                    .Select(x => x.UserId)
                    .ToListAsync(cancellationToken);

                var events = await _context.Set<Event>()
                    .Include(x => x.EventCategory)
                    .Include(y => y.Participants)
                    .Include(z => z.Posts)
                    .Where(e => userFriendsIds.Contains(e.OwnerId) && !e.IsPublic)
                    .ToListAsync(cancellationToken);

                var users = await _context.Set<UserProfile>().ToListAsync();

                return new GetFriendsEventsResponse(
                    events.Select(e => new EventDto(
                        e.Id,
                        users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().Username,
                        users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().ProfilePicture.Value) : null,
                        e.EventName,
                        e.EventDescription,
                        e.TakenPlacesAmount,
                        e.MaxAmountOfPeople,
                        e.Place,
                        e.Location.X,
                        e.Location.Y,
                        e.StartDate,
                        e.Cost,
                        e.MinAge,
                        e.EventCategory.CategoryName,
                        e.EventCategory.ImageId.HasValue ? _azureStorageService.GetReadFileToken(e.EventCategory.ImageId.Value) : null,
                        e.Participants.Select(y => new UserDto(y.UserId, y.Username, y.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(y.ProfilePicture.Value) : null)),
                        e.Posts.Select(p => new PostDto(p.Id, p.AuthorName, p.PostName, p.PublishedDate, p.PostText)))).ToList());
            }
        }
    }
}
