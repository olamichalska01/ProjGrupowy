using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using ComUnity.Application.Infrastructure.Services;
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
        public async Task<IActionResult> AddEventCategory([FromBody] AddEventCategoryCommand command)
        {
            await Mediator.Send(command);

            return StatusCode(StatusCodes.Status201Created);
        }

        public record AddEventCategoryCommand(string Name, Guid? ImageId) : IRequest<AddEventCategoryResponse>;

        public record AddEventCategoryResponse(string PictureUrl);

        public class AddEventCategoryCommandValidator : AbstractValidator<AddEventCategoryCommand>
        {
            public AddEventCategoryCommandValidator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MinimumLength(3);
            }
        }

        internal class AddEventCategoryCommandHandler : IRequestHandler<AddEventCategoryCommand, AddEventCategoryResponse>
        {
            private readonly ComUnityContext _context;
            private readonly IAzureStorageService _azureStorageService;

            public AddEventCategoryCommandHandler(ComUnityContext context, IAzureStorageService azureStorageService, IAuthenticatedUserProvider authenticatedUserProvider)
            {
                _context = context;
                _azureStorageService = azureStorageService;
            }

            public async Task<AddEventCategoryResponse> Handle(AddEventCategoryCommand request, CancellationToken cancellationToken)
            {
                var eventCategory = await _context.Set<EventCategory>().FirstOrDefaultAsync(e => e.CategoryName == request.Name, cancellationToken);

                if (eventCategory != null)
                {
                    throw new EventCategoryAlreadyExistException();
                }

                eventCategory = new EventCategory(request.Name);
                
                if(request.ImageId != null)
                {
                    eventCategory.SetCategoryImage((Guid)request.ImageId);
                }

                await _context.AddAsync(eventCategory);
                await _context.SaveChangesAsync();

                var pictureUrl = _azureStorageService.GetReadFileToken((Guid)request.ImageId);

                return new AddEventCategoryResponse(pictureUrl);
            }
        }
    }
}
