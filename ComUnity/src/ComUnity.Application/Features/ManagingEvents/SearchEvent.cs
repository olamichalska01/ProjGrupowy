using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.ManagingEvents;

public class SearchEvent : ApiControllerBase
{
    [HttpGet("/api/events/{searchText}")]
    public async Task<ActionResult<GetEventsResponse>> GetEvents([FromRoute] string searchText)
    {
        return await Mediator.Send(new GetEventsQuery(searchText));
    }

    public record GetEventsQuery(string searchText) : IRequest<GetEventsResponse>;

    public record GetEventsResponse(ICollection<EventDto> Events);

    internal class GetEventByIdQueryHandler : IRequestHandler<GetEventsQuery, GetEventsResponse>
    {
        private readonly ComUnityContext _context;

        public GetEventByIdQueryHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<GetEventsResponse> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.Set<Event>()
                .Where(e => e.EventName.ToLower().Contains(request.searchText.ToLower())
                ||
                e.Place.ToLower().Contains(request.searchText.ToLower()))
                .ToListAsync();

            return new GetEventsResponse(
                events.Select(e => new EventDto(
                    e.Id,
                    e.EventName,
                    e.MaxAmountOfPeople,
                    e.Place,
                    e.EventDate,
                    e.Cost,
                    e.MinAge,
                    e.EventCategory.CategoryName)).ToList());
        }
    }
}


