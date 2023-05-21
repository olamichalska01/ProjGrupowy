using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.ManagingEvents;

public class AddEventCategoryController : ApiControllerBase
{
    [HttpPost("/api/event-categories/")]
    public async Task<IActionResult> AddEventCategory([FromBody] AddEventCategoryCommand command)
    {
        await Mediator.Send(command);

        return StatusCode(StatusCodes.Status201Created);
    }

    public record AddEventCategoryCommand(string Name) : IRequest<Unit>;

    public class AddEventCategoryCommandValidator : AbstractValidator<AddEventCategoryCommand>
    {
        public AddEventCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(3);
        }
    }

    internal class AddEventCategoryCommandHandler : IRequestHandler<AddEventCategoryCommand, Unit>
    {
        private readonly ComUnityContext _context;

        public AddEventCategoryCommandHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(AddEventCategoryCommand request, CancellationToken cancellationToken)
        {
            var eventCategory = await _context.Set<EventCategory>().FirstOrDefaultAsync(e => e.CategoryName == request.Name, cancellationToken);

            if (eventCategory != null)
            {
                throw new EventCategoryAlreadyExistException();
            }

            eventCategory = new EventCategory(request.Name);

            await _context.AddAsync(eventCategory);
            await _context.SaveChangesAsync();

            return Unit.Value;
        }
    }
}
