using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Dtos;
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

            public FilterEventsQueryHandler(ComUnityContext context)
            {
                _context = context;
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
                    query = query.Where(x => request.MinAge >= x.MinAge);
                }

                if (request.FromDate > DateTime.MinValue)
                {
                    query = query.Where(x => request.FromDate <= x.EventDate);
                }

                if (request.ToDate > DateTime.MinValue)
                {
                    query = query.Where(x => request.ToDate >= x.EventDate);
                }

                var events = await query.ToListAsync(cancellationToken);

                return new FilterEventsResponse(
                    events.Select(e => new EventDto(
                        e.Id,
                        e.EventName,
                        e.TakenPlacesAmount,
                        e.MaxAmountOfPeople,
                        e.Place,
                        e.EventDate,
                        e.Cost,
                        e.MinAge,
                        e.EventCategory.CategoryName,
                        e.Participants.Select(y => new UserDto(y.UserId, y.Username)))).ToList());
            }
        }
    }
}
