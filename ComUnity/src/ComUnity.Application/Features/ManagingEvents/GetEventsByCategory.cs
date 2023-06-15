using ComUnity.Application.Common;
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
        private readonly IAzureStorageService _azureStorageService;

        public GetEventByIdQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService)
        {
            _context = context;
            _azureStorageService = azureStorageService;
        }

        public async Task<GetEventsByCategoryResponse> Handle(GetEventsByCategoryQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.Set<Event>()
                .Include(x => x.EventCategory)
                .Where(x => x.EventCategory.CategoryName == request.CategoryName)
                .ToListAsync(cancellationToken);
            var users = await _context.Set<UserProfile>().ToListAsync();

            return new GetEventsByCategoryResponse(
                events.Select(e => new EventDto(
                    e.Id,
                    users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().Username,
                    users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().ProfilePicture.Value) : null,
                    e.EventName,
                    e.EventDescription,
                    e.TakenPlacesAmount,
                    e.MaxAmountOfPeople,
                    e.Place,
                    e.Location.X,
                    e.Location.Y,
                    e.StartDate,
                    e.EndDate,
                    e.Cost,
                    e.MinAge,
                    e.EventCategory.CategoryName,
                    e.EventCategory.ImageId.HasValue ? _azureStorageService.GetReadFileToken(e.EventCategory.ImageId.Value) : null,
                    e.Participants.Select(y => new UserDto(y.UserId, y.Username, y.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(y.ProfilePicture.Value) : null)),
                    e.Posts.Select(p => new PostDto(p.Id, p.AuthorName, p.PostName, p.PublishedDate, p.PostText)))).ToList());
        }
    }
}
