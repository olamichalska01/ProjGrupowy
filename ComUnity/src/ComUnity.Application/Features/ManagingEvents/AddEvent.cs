using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using ComUnity.Application.Infrastructure.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace ComUnity.Application.Features.ManagingEvents;

public class AddEventController : ApiControllerBase
{
    [HttpPost("/api/events")]
    [ProducesResponseType(typeof(AddEventResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AddEventResponse>> AddEvent([FromBody] AddEventCommand command)
    {
        return await Mediator.Send(command);
    }

    public record AddEventCommand(
        string EventName,
        int MaxAmountOfPeople,
        string Place,
        double Latitude,
        double Longitude,
        DateTime EventDate,
        double Cost, 
        int MinAge,
        string EventCategory)
        : IRequest<AddEventResponse>;

    public record AddEventResponse(
        Guid Id,
        string EventName,
        int MaxAmountOfPeople,
        string Place,
        DateTime EventDate,
        double Cost,
        int MinAge,
        string EventCategory)
        : IRequest<AddEventResponse>;

    public class AddEventValidator : AbstractValidator<AddEventCommand>
    {
        public AddEventValidator()
        {
            RuleFor(x => x.EventName).NotEmpty();
            RuleFor(x => x.MaxAmountOfPeople).NotEmpty();
            RuleFor(x => x.Place).NotEmpty();
            RuleFor(x => x.EventDate).NotEmpty();
            RuleFor(x => x.Cost).NotEmpty();
            RuleFor(x => x.MinAge).NotEmpty();
            RuleFor(x => x.EventCategory).NotEmpty();
        }
    }

    internal class AddEventHandler : IRequestHandler<AddEventCommand, AddEventResponse>
    {
        private readonly ComUnityContext _context;
        private readonly IAuthenticatedUserProvider _authenticatedUserProvider;

        public AddEventHandler(ComUnityContext context, IAuthenticatedUserProvider authenticatedUserProvider)
        {
            _context = context;
            _authenticatedUserProvider = authenticatedUserProvider;
        }

        public async Task<AddEventResponse> Handle(AddEventCommand request, CancellationToken cancellationToken)
        {
            var eCategory = await _context.Set<EventCategory>().FirstOrDefaultAsync(ec => ec.CategoryName == request.EventCategory, cancellationToken);

            if (eCategory == null)
            {
                throw new EventCategoryDoesNotExistException(request.EventCategory);
            }

            var newEventId = NewId.NextGuid();
            var userId = _authenticatedUserProvider.GetUserId();

            var e = new Event(
                newEventId,
                userId,
                request.EventName,
                request.MaxAmountOfPeople,
                request.Place,
                new Point(request.Latitude, request.Longitude) { SRID = 4326 },
                request.EventDate,
                request.Cost,
                request.MinAge,
                eCategory);

            await _context.AddAsync(e, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddEventResponse(
                e.Id,
                e.EventName,
                e.MaxAmountOfPeople,
                e.Place,
                e.EventDate,
                e.Cost,
                e.MinAge,
                e.EventCategory.CategoryName);
        }
    }
}
