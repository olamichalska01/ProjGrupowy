using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.ManagingEvents;

public class AddPostController : ApiControllerBase
{
    [HttpPost("/api/events/{eventId}/addPost")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> AddPost([FromRoute] Guid eventId, [FromBody] AddPostCommand command, CancellationToken cancellationToken)
    {
        var addPostCommand = command with {  EventId = eventId };
        await Mediator.Send(addPostCommand);


        return NoContent();
    }

    public record AddPostCommand(
        Guid EventId,
        string PostName,
        string PostText,
        DateTime Date)
        : IRequest<Unit>;

    public class AddPostValidator : AbstractValidator<AddPostCommand>
    {
        public AddPostValidator()
        {
            RuleFor(x => x.PostName).NotEmpty();
            RuleFor(x => x.PostText).NotEmpty();
        }
    }

    internal class AddPostHandler : IRequestHandler<AddPostCommand, Unit>
    {
        private readonly ComUnityContext _context;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public AddPostHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
        {
            _context = context;
            _authenticatedUserProvider = authenticatedUserProvider;
        }

        public async Task<Unit> Handle(AddPostCommand request, CancellationToken cancellationToken)
        {
            var newPostId = NewId.NextGuid();
            var userId = _authenticatedUserProvider.GetUserId();
            var user = await _context.Set<UserProfile>().FirstOrDefaultAsync(u => u.UserId == userId);

            var eventId = request.EventId;
            var eventObject = await _context.Set<Event>().FirstOrDefaultAsync(e => e.Id == eventId);

            var date = DateTime.UtcNow;

            var post = new Post(
                newPostId,
                user.Username,
                userId,
                request.PostName,
                date,
                request.PostText);

            eventObject.AddPost(post);

            await _context.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
