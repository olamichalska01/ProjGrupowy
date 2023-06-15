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
    public class FilterEventsController : ApiControllerBase
    {
        [HttpGet("/api/events/filter/")]
        [ProducesResponseType(typeof(FilterEventsResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<FilterEventsResponse>> GetEvents(
            [FromQuery] string? categoryName, 
            [FromQuery] int costMin, 
            [FromQuery] int costMax, 
            [FromQuery] int amountOfPeople,
            [FromQuery] int minAge,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate
            )
        {
            return await Mediator.Send(new FilterEventsQuery(categoryName, costMin, costMax, amountOfPeople, minAge, fromDate, toDate));
        }

        public record FilterEventsQuery(string? CategoryName, int CostMin, int CostMax, 
            int AmountOfPeople, int MinAge, DateTime FromDate, DateTime ToDate) : IRequest<FilterEventsResponse>;

        public record FilterEventsResponse(ICollection<EventDto> Events);

        internal class FilterEventsQueryHandler : IRequestHandler<FilterEventsQuery, FilterEventsResponse>
        {
            private readonly ComUnityContext _context;
            private readonly IAzureStorageService _azureStorageService;


            public FilterEventsQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService)
            {
                _context = context;
                _azureStorageService = azureStorageService;
            }

            public async Task<FilterEventsResponse> Handle(FilterEventsQuery request, CancellationToken cancellationToken)
            {
                var query = _context.Set<Event>()
                     .Include(x => x.EventCategory).Where(x=> x.Id != null);

                if(request.CategoryName != null)
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

                return new FilterEventsResponse(
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
                        e.EndDate,
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
