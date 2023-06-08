using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static ComUnity.Application.Features.ManagingEvents.GetEventByIdController;

namespace ComUnity.Application.Features.ManagingEvents;

public class GetEventsCategoriesController : ApiControllerBase
{
    [HttpGet("/api/events-categories/")]
    [ProducesResponseType(typeof(GetEventsCategoriesResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetEventsCategoriesResponse>> GetEvents()
    {
        return await Mediator.Send(new GetEventsCategoriesQuery());
    }

    public record GetEventsCategoriesQuery() : IRequest<GetEventsCategoriesResponse>;

    public record GetEventsCategoriesResponse(ICollection<GetEventsCategoriesResponse.EventCategory> Categories)
    {
        public record EventCategory(string Name, string? ImageId);
    };

    internal class GetEventsCategoriesQueryHandler : IRequestHandler<GetEventsCategoriesQuery, GetEventsCategoriesResponse>
    {
        private readonly ComUnityContext _context;
        private readonly IAzureStorageService _azureStorageService;

        public GetEventsCategoriesQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService)
        {
            _context = context;
            _azureStorageService = azureStorageService;
        }

        public async Task<GetEventsCategoriesResponse> Handle(GetEventsCategoriesQuery request, CancellationToken cancellationToken)
        {
            var eventCategories = await _context.Set<EventCategory>().ToListAsync(cancellationToken);
            return new GetEventsCategoriesResponse(
                eventCategories.Select(category =>
                  new GetEventsCategoriesResponse.EventCategory(category.CategoryName, category.ImageId.HasValue ? _azureStorageService.GetReadFileToken(category.ImageId.Value) : null)).ToList());
        }
    }
}
