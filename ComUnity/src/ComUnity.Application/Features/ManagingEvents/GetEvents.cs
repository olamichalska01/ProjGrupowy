using ComUnity.Application.Common;
using ComUnity.Application.Common.Models;
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
using static ComUnity.Application.Features.ManagingEvents.GetEventsCategoriesController;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventsController : ApiControllerBase
{
    [HttpGet("/api/events")]
    [ProducesResponseType(typeof(GetEventsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetEventsResponse>> GetEvents()
    {
        return await Mediator.Send(new GetEventsQuery());
    }

    public record GetEventsQuery() : IRequest<GetEventsResponse>;

    public record GetEventsResponse(ICollection<EventDto> Events);

    internal class GetEventByIdQueryHandler : IRequestHandler<GetEventsQuery, GetEventsResponse>
    {
        private readonly ComUnityContext _context;
        private readonly IAzureStorageService _azureStorageService;

        public GetEventByIdQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService)
        {
            _context = context;
            _azureStorageService = azureStorageService;
        }

        public async Task<GetEventsResponse> Handle(GetEventsQuery request, CancellationToken cancellationToken)
        {
            var events = await _context.Set<Event>()
                .Include(x => x.EventCategory)
                .Include(y => y.Participants)
                .ToListAsync(cancellationToken);
            var users = await _context.Set<UserProfile>().ToListAsync();

            return new GetEventsResponse(
                events.Select(e => new EventDto(
                    e.Id,
                    users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().Username,
                    users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(users.Where(u => u.UserId == e.OwnerId).FirstOrDefault().ProfilePicture.Value) : null,
                    e.EventName,
                    e.TakenPlacesAmount,
                    e.MaxAmountOfPeople,
                    e.Place,
                    e.Location.X,
                    e.Location.Y,
                    e.EventDate,
                    e.Cost,
                    e.MinAge,
                    e.EventCategory.CategoryName,
                    e.EventCategory.ImageId.HasValue ? _azureStorageService.GetReadFileToken(e.EventCategory.ImageId.Value) : null,
                    e.Participants.Select(y => new UserDto(y.UserId, y.Username, y.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(y.ProfilePicture.Value) : null)))).ToList());
        }
    }
}
