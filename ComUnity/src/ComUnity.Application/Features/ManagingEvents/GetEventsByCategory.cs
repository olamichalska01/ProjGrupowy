using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ComUnity.Application.Features.ManagingEvents.GetEventsController;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventsByCategoryController : ApiControllerBase
{
    [HttpGet("/api/events/by-category/")]
    [ProducesResponseType(typeof(GetEventsByCategoryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetEventsByCategoryResponse>> GetEvents([FromQuery] string categoryName)
    {
        return await Mediator.Send(new GetEventsByCategoryQuery(categoryName));
    }

    public record GetEventsByCategoryQuery(string CategoryName) : IRequest<GetEventsByCategoryResponse>;

    public record GetEventsByCategoryResponse(ICollection<EventDto> Events);

    internal class GetEventByIdQueryHandler : IRequestHandler<GetEventsByCategoryQuery, GetEventsByCategoryResponse>
    {
        private readonly ComUnityContext _context;

        public GetEventByIdQueryHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<GetEventsByCategoryResponse> Handle(GetEventsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.Set<Event>()
                .Include(x => x.EventCategory)
                .Where(x => x.EventCategory.CategoryName == request.CategoryName)
                .ToListAsync(cancellationToken);

            return new GetEventsByCategoryResponse(
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
