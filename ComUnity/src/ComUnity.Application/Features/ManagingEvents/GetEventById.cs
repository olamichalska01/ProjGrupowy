using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventByIdController : ApiControllerBase
{
    [HttpGet("/api/events/{id}")]
    public async Task<ActionResult<GetEventByIdResponse>> GetEventById([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetEventByIdQuery(id));
    }

    public record GetEventByIdQuery(Guid Id) : IRequest<GetEventByIdResponse>;

    public record GetEventByIdResponse(EventDto EventDetails);

    internal class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdResponse>
    {
        private readonly ComUnityContext _context;

        public GetEventByIdQueryHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<GetEventByIdResponse> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Set<Event>().Include(x => x.EventCategory).FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(nameof(Event), request.Id);
            }

            return new GetEventByIdResponse(new EventDto(
                    result.Id,
                    result.EventName,
                    result.MaxAmountOfPeople,
                    result.Place,
                    result.EventDate,
                    result.Cost,
                    result.MinAge,
                    result.EventCategory.CategoryName));
        }
    }
}
