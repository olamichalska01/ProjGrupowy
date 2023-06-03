using ComUnity.Application.Common;
using ComUnity.Application.Common.Utils;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Dtos;
using ComUnity.Application.Features.ManagingEvents.Entities;
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

    public SearchEventQueryHandler(ComUnityContext context)
    {
        _context = context;
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
            .Where(x => x.EventDate > DateTime.UtcNow);

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
                query = query.OrderByDirection(request.Sort.Direction.ToLowerInvariant(), x => x.EventDate);
            }
            else if (request.Sort.SortBy == SortableProperties.Distance)
            {
                query = query.OrderByDirection(request.Sort.Direction.ToLowerInvariant(), x => x.Location.Distance(requestLocation));
            }
        }

        var total = await query.CountAsync(cancellationToken);
        var results = await query
            .Skip(skipRows)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new SearchEventsResponse(
            TotalCount: total, 
            Results: results.Select(x => new EventDto(
                x.Id,
                x.EventName,
                x.MaxAmountOfPeople,
                x.Place,
                x.EventDate,
                x.Cost,
                x.MinAge,
                "football")).ToList());
    }
}
