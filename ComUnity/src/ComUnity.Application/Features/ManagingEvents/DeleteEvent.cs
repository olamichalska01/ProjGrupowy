using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ComUnity.Application.Features.ManagingEvents.AddEventController;

namespace ComUnity.Application.Features.ManagingEvents;

public class DeleteEventController : ApiControllerBase
{
    [HttpDelete("/api/events/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddEvent([FromRoute] Guid id)
    {
        await Mediator.Send(new DeleteEventCommand(id));

        return NoContent();
    }

    public record DeleteEventCommand(Guid Id) : IRequest<Unit>;

    internal class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, Unit>
    {
        private readonly ComUnityContext _context;

        public DeleteEventCommandHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            var e = await _context.Set<Event>().FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (e == null)
            {
                throw new NotFoundException(nameof(Event), request.Id);
            }

            _context.Remove(e);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
