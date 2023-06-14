using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using ComUnity.Application.Features.UserProfileManagement;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace ComUnity.Application.Features.ManagingEvents;

public class AddPostController : ApiControllerBase
{
    [HttpPost("/api/events/{eventId}/addPost")]
    [ProducesResponseType(typeof(AddPostResponse), StatusCodes.Status200OK)]
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
        string AuthorName,
        DateTime Date)
        : IRequest<AddPostResponse>;

    public record AddPostResponse(
        Guid id, 
        Guid EventId,
        string PostName,
        string PostText,
        DateTime Date,
        string AuthorName
        )
        : IRequest<AddPostResponse>;

    public class AddPostValidator : AbstractValidator<AddPostCommand>
    {
        public AddPostValidator()
        {
            RuleFor(x => x.PostName).NotEmpty();
            RuleFor(x => x.PostText).NotEmpty();
            RuleFor(x => x.AuthorName).NotEmpty();
            RuleFor(x => x.Date).NotEmpty();
        }
    }

    internal class AddPostHandler : IRequestHandler<AddPostCommand, AddPostResponse>
    {
        private readonly ComUnityContext _context;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public AddPostHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
        {
            _context = context;
            _authenticatedUserProvider = authenticatedUserProvider;
        }

        public async Task<AddPostResponse> Handle(AddPostCommand request, CancellationToken cancellationToken)
        {

            var newPostId = NewId.NextGuid();
            var userId = _authenticatedUserProvider.GetUserId();
            var user = await _context.Set<UserProfile>().FirstOrDefaultAsync(u => u.UserId == userId);

            var eventId = request.EventId;
            var eventObject = await _context.Set<Event>().FirstOrDefaultAsync(e => e.Id == eventId);

            var post = new Post(
                newPostId,
                user.Username,
                userId,
                request.PostName,
                request.Date,
                request.PostText);

            eventObject.AddPost(post);

            await _context.AddAsync(post, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddPostResponse(
                post.Id,
                eventId,
                post.PostName,
                post.PostText,
                post.PublishedDate,
                post.AuthorName
                );
        }
    }
}
