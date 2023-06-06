using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ComUnity.Application.Features.ManagingEvents.GetEventsCategoriesController;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventsController : ApiControllerBase
{
    [HttpGet("/api/events")]
    [ProducesResponseType(typeof(GetEventsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetEventsResponse>> GetEvents()
    {
        return await Mediator.Send(new GetEventsQuery());
    }

    public record GetEventsQuery() : IRequest<GetEventsResponse>;

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
            var events = await _context.Set<Event>().Include(x => x.EventCategory).Include(y => y.Participants).ToListAsync(cancellationToken);

            return new GetEventsResponse(
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
