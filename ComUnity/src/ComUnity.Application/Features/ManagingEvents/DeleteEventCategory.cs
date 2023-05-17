using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.ManagingEvents;

public class DeleteEventCategoryController : ApiControllerBase
{
    [HttpDelete("/api/event-categories/{name}")]
    public async Task<IActionResult> AddEvent([FromRoute] string name)
    {
        await Mediator.Send(new DeleteEventCategoryCommand(name));

        return NoContent();
    }

    public record DeleteEventCategoryCommand(string Name) : IRequest<Unit>;

    internal class DeleteEventCategoryCommandHandler : IRequestHandler<DeleteEventCategoryCommand,  Unit>
    {
        private readonly ComUnityContext _context;

        public DeleteEventCategoryCommandHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Set<EventCategory>().FirstOrDefaultAsync(e => e.CategoryName == request.Name, cancellationToken);

            if (category == null)
            {
                throw new NotFoundException(nameof(EventCategory), request.Name);
            }

            _context.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
