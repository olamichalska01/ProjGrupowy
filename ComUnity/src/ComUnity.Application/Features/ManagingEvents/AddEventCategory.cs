using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using static ComUnity.Application.Features.ManagingEvents.AddEventCategoryController;

namespace ComUnity.Application.Features.ManagingEvents
{
    public class AddEventCategoryController : ApiControllerBase
    {

        [HttpPost("/api/event-categories/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddEventCategory([FromForm] AddEventCategoryCommand command)
        {
            await Mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created);
        }

        public record AddEventCategoryCommand(string Name, IFormFile Image) : IRequest<Unit>;

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
            private string ImageUploadPath = "..\\ComUnity.Application\\Common\\Images\\"; 

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

                if (request.Image != null && request.Image.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(request.Image.FileName)}";
                    var filePath = Path.Combine(ImageUploadPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Image.CopyToAsync(stream);
                    }

                    eventCategory.ImagePath = filePath; 
                }

                await _context.AddAsync(eventCategory);
                await _context.SaveChangesAsync();

                return Unit.Value;
            }
        }
    }
}
