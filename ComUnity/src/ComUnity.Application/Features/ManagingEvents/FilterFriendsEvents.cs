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
    public class FilterFriendsEventsController : ApiControllerBase
    {
        [HttpGet("/api/events/filterFriendsEvents/")]
        [ProducesResponseType(typeof(FilterFriendsEventsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<FilterFriendsEventsResponse>> GetEvents(
            [FromQuery] string? categoryName,
            [FromQuery] int costMin,
            [FromQuery] int costMax,
            [FromQuery] int amountOfPeople,
            [FromQuery] int minAge,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate
            )
        {
            return await Mediator.Send(new FilterFriendsEventsQuery(categoryName, costMin, costMax, amountOfPeople, minAge, fromDate, toDate));
        }

        public record FilterFriendsEventsQuery(string? CategoryName, int CostMin, int CostMax,
            int AmountOfPeople, int MinAge, DateTime FromDate, DateTime ToDate) : IRequest<FilterFriendsEventsResponse>;

        public record FilterFriendsEventsResponse(ICollection<EventDto> Events);

        internal class FilterEventsQueryHandler : IRequestHandler<FilterFriendsEventsQuery, FilterFriendsEventsResponse>
        {
            private readonly ComUnityContext _context;
            private readonly IAzureStorageService _azureStorageService;
            private readonly IAuthenticatedUserProvider _authenticatedUserProvider;


            public FilterEventsQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService, IAuthenticatedUserProvider authenticatedUserProvider)
            {
                _context = context;
                _azureStorageService = azureStorageService;
                _authenticatedUserProvider = authenticatedUserProvider;
            }

            public async Task<FilterFriendsEventsResponse> Handle(FilterFriendsEventsQuery request, CancellationToken cancellationToken)
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

                var query = _context.Set<Event>()
                    .Include(x => x.EventCategory)
                    .Include(y => y.Participants)
                    .Include(z => z.Posts)
                    .Where(e => userFriendsIds.Contains(e.OwnerId) && !e.IsPublic);

                if (request.CategoryName != null)
                {
                    query = query.Where(x => x.EventCategory.CategoryName == request.CategoryName);

                }

                if (request.CostMin > 0)
                {
                    query = query.Where(x => x.Cost >= request.CostMin);
                }

                if (request.CostMax > 0)
                {
                    query = query.Where(x => x.Cost <= request.CostMax);
                }

                if (request.AmountOfPeople > 0)
                {
                    query = query.Where(x => request.AmountOfPeople <= x.MaxAmountOfPeople - x.TakenPlacesAmount);
                }

                if (request.MinAge > 0)
                {
                    query = query.Where(x => request.MinAge <= x.MinAge);
                }

                if (request.FromDate > DateTime.MinValue)
                {
                    query = query.Where(x => request.FromDate <= x.StartDate);
                }

                if (request.ToDate > DateTime.MinValue)
                {
                    query = query.Where(x => request.ToDate >= x.StartDate);
                }

                var events = await query.Include(x => x.Owner).ToListAsync(cancellationToken);

                return new FilterFriendsEventsResponse(
                    events.Select(e => new EventDto(
                        e.Id,
                        e.Owner.Username,
                        e.Owner.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(e.Owner.ProfilePicture.Value) : null,
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
