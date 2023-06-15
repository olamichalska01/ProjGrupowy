using ComUnity.Application.Common;
using ComUnity.Application.Common.Utils;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.UserProfileManagement.Dtos;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static ComUnity.Application.Features.ManagingEvents.GetEventsByCategoryController;
using static ComUnity.Application.Features.ManagingEvents.SearchEventsQuery;

namespace ComUnity.Application.Features.ManagingEvents;

public class SearchEventsController : ApiControllerBase
{
    [HttpGet("/api/events/search")]
    [ProducesResponseType(typeof(SearchEventsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SearchEventsResponse>> SearchEvents([FromQuery] SearchEventsQuery query)
    {
        return await Mediator.Send(query);
    }
}

public static class SortableProperties
{
    public static string Distance = "Distance";

    public static string Date = "Date";
}

public record SearchEventsQuery(
    PageInfo? Page,
    SortInfo? Sort,
    Filter? Filters,
    double Latitude,
    double Longitude) : IRequest<SearchEventsResponse>
{
    public record PageInfo(int? PageIndex, int? PageSize);

    public record SortInfo(string SortBy, string Direction);

    public record Filter(
        ICollection<Guid>? SelectedCategoriesIds,
        double? MaxDistance,
        string? EventName,
        bool? ShowPublicEvents);
}

public record SearchEventsResponse(int TotalCount, ICollection<EventDto> Results);

public class SearchEventsQueryValidator : AbstractValidator<SearchEventsQuery>
{
    public SearchEventsQueryValidator()
    {
    }
}

internal class SearchEventQueryHandler : IRequestHandler<SearchEventsQuery, SearchEventsResponse>
{
    private static int DefaultPageSize = 20;

    private readonly ComUnityContext _context;
    private readonly IAzureStorageService _azureStorageService;

    public SearchEventQueryHandler(ComUnityContext context, IAzureStorageService azureStorageService)
    {
        _context = context;
        _azureStorageService = azureStorageService;
    }

    public async Task<SearchEventsResponse> Handle(SearchEventsQuery request, CancellationToken cancellationToken)
    {
        var gf = NetTopologySuite.NtsGeometryServices.Instance.CreateGeometryFactory(4326);

        var requestLocation = gf.CreatePoint(new Coordinate(request.Latitude, request.Longitude));

        var pageSize = request.Page?.PageSize ?? DefaultPageSize;
        var skipRows = request.Page?.PageIndex ?? 0 * pageSize;

        var query = _context.Set<Event>()
            .AsNoTracking()
            .Include(x => x.EventCategory)
            .Where(x => x.StartDate > DateTime.UtcNow);

        if (request.Filters is not null)
        {
            var maxDistanceInMeters = request.Filters.MaxDistance.GetValueOrDefault() * 1000;

            query = query
                .WhereIf(!string.IsNullOrEmpty(request.Filters.EventName), x => EF.Functions.Like(x.EventName, $"{request.Filters!.EventName}%"))
                // Categories
                .WhereIf(request.Filters.SelectedCategoriesIds is not null && request.Filters.SelectedCategoriesIds.Count > 0,
                    x => request.Filters.SelectedCategoriesIds!.Contains(x.EventCategory.Id))
                // Public Events
                .WhereIf(request.Filters.ShowPublicEvents.HasValue && request.Filters.ShowPublicEvents.Value,
                    x => x.IsPublic)
                // Distance
                .WhereIf(maxDistanceInMeters > 0, x => x.Location.IsWithinDistance(requestLocation, maxDistanceInMeters));
        }

        if (request.Sort is not null)
        {
            if(request.Sort.SortBy == SortableProperties.Date)
            {
                query = query.OrderByDirection(request.Sort.Direction.ToLowerInvariant(), x => x.StartDate);
            }
            else if (request.Sort.SortBy == SortableProperties.Distance)
            {
                query = query.OrderByDirection(request.Sort.Direction.ToLowerInvariant(), x => x.Location.Distance(requestLocation));
            }
        }

        var total = await query.CountAsync(cancellationToken);
        var results = await query
            .Include(x => x.Owner)
            .Skip(skipRows)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new SearchEventsResponse(
            TotalCount: total, 
            Results: results.Select(x => new EventDto(
                x.Id,
                x.Owner.Username,
                x.Owner.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(x.Owner.ProfilePicture.Value) : null,
                x.EventName,
                x.EventDescription,
                x.TakenPlacesAmount,
                x.MaxAmountOfPeople,
                x.Place,
                x.Location.X,
                x.Location.Y,
                x.StartDate,
                x.EndDate,
                x.Cost,
                x.MinAge,
                x.EventCategory.CategoryName,
                x.EventCategory.ImageId.HasValue ? _azureStorageService.GetReadFileToken(x.EventCategory.ImageId.Value) : null,
                x.Participants.Select(y => new UserDto(y.UserId, y.Username, y.ProfilePicture.HasValue ? _azureStorageService.GetReadFileToken(y.ProfilePicture.Value) : null)),
                x.Posts.Select(p => new PostDto(p.Id, p.AuthorName, p.PostName, p.PublishedDate, p.PostText)))).ToList());
    }
}
