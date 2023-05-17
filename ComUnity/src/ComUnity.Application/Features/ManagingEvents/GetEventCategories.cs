using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventsCategoriesController : ApiControllerBase
{
    [HttpGet("/api/events-categories/")]
    public async Task<ActionResult<GetEventsCategoriesResponse>> GetEvents()
    {
        return await Mediator.Send(new GetEventsCategoriesQuery());
    }

    public record GetEventsCategoriesQuery() : IRequest<GetEventsCategoriesResponse>;

    public record GetEventsCategoriesResponse(ICollection<GetEventsCategoriesResponse.EventCategory> Categories)
    {
        public record EventCategory(string Name);
    };

    internal class GetEventsCategoriesQueryHandler : IRequestHandler<GetEventsCategoriesQuery, GetEventsCategoriesResponse>
    {
        private readonly ComUnityContext _context;

        public GetEventsCategoriesQueryHandler(ComUnityContext context)
        {
            _context = context;
        }

        public async Task<GetEventsCategoriesResponse> Handle(GetEventsCategoriesQuery request, CancellationToken cancellationToken)
        {
            var eventCategories = await _context.Set<EventCategory>().ToListAsync(cancellationToken);

            return new GetEventsCategoriesResponse(
                eventCategories.Select(category =>
                  new GetEventsCategoriesResponse.EventCategory(category.CategoryName)).ToList());
        }
    }
}
