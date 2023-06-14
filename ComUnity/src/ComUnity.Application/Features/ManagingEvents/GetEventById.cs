using ComUnity.Application.Common;
using ComUnity.Application.Common.Exceptions;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Dtos;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static ComUnity.Application.Features.ManagingEvents.FilterEventsController;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventByIdController : ApiControllerBase
{
    [HttpGet("/api/events/{id}")]
    [ProducesResponseType(typeof(GetEventByIdResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetEventByIdResponse>> GetEventById([FromRoute] Guid id)
    {
        return await Mediator.Send(new GetEventByIdQuery(id));
    }

    public record GetEventByIdQuery(Guid Id) : IRequest<GetEventByIdResponse>;

    public record GetEventByIdResponse(EventDto EventDetails);

    internal class GetEventByIdQueryHandler : IRequestHandler<GetEventByIdQuery, GetEventByIdResponse>
    {
        private readonly ComUnityContext _context;
        private readonly IAzureStorageService _azureStorageService;

        public GetEventByIdQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService)
        {
            _context = context;
            _azureStorageService = azureStorageService;
        }

        public async Task<GetEventByIdResponse> Handle(GetEventByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _context.Set<Event>()
                .Include(x => x.EventCategory)
                .Include(y => y.Participants)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (result == null)
            {
                throw new NotFoundException(nameof(Event), request.Id);
            }

            var users = await _context.Set<UserProfile>().ToListAsync();

            return new GetEventByIdResponse(new EventDto(
                    result.Id,
                    users.Where(u => u.UserId == result.OwnerId).FirstOrDefault().Username,
                    users.Where(u => u.UserId == result.OwnerId).FirstOrDefault().ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(users.Where(u => u.UserId == result.OwnerId).FirstOrDefault().ProfilePicture.Value) : null,
                    result.EventName,
                    result.TakenPlacesAmount,
                    result.MaxAmountOfPeople,
                    result.Place,
                    result.Location.X,
                    result.Location.Y,
                    result.StartDate,
                    result.Cost,
                    result.MinAge,
                    result.EventCategory.CategoryName,
                    result.EventCategory.ImageId.HasValue ? _azureStorageService.GetReadFileToken(result.EventCategory.ImageId.Value) : null,
                    result.Participants.Select(y => new UserDto(y.UserId, y.Username, y.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(y.ProfilePicture.Value) : null)),
                    result.Posts.Select(p => new PostDto(p.Id, p.AuthorName, p.PostName, p.PublishedDate, p.PostText))));
        }
    }
}
