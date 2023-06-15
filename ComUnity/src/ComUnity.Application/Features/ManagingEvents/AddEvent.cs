using ComUnity.Application.Common;
using ComUnity.Application.Database;
using ComUnity.Application.Features.ManagingEvents.Entities;
using ComUnity.Application.Features.ManagingEvents.Exceptions;
using ComUnity.Application.Features.UserProfileManagement.Entities;
using ComUnity.Application.Infrastructure.Services;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections;

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
        string Description,
        int MaxAmountOfPeople,
        string Place,
        double Latitude,
        double Longitude,
        DateTime StartDate,
        DateTime EndDate,
        double Cost, 
        int MinAge,
        string EventCategory,
        bool IsPublic)
        : IRequest<AddEventResponse>;

    public record AddEventResponse(
        Guid Id,
        string EventName,
        string Description,
        int TakenPlaces,
        int MaxAmountOfPeople,
        string Place,
        DateTime EventDate,
        double? Cost,
        int MinAge,
        string EventCategory,
        string UserName
        )
        : IRequest<AddEventResponse>;

    public class AddEventValidator : AbstractValidator<AddEventCommand>
    {
        public AddEventValidator()
        {
            RuleFor(x => x.EventName).NotEmpty();
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Description).Must(x => x.Length <= 1000);
            RuleFor(x => x.MaxAmountOfPeople).NotEmpty();
            RuleFor(x => x.Place).NotEmpty();
            RuleFor(x => x.StartDate)
                .NotEmpty()
                .Must(x => x >= DateTime.Now).WithMessage("Start date have to be in the future."); ;
            RuleFor(x => x.EndDate)
                .NotEmpty()
                .Must((model, x) => x >= model.StartDate).WithMessage("End date have to after start date.");
            RuleFor(x => x.Cost).GreaterThanOrEqualTo(0);
            RuleFor(x => x.MinAge).NotEmpty();
            RuleFor(x => x.EventCategory).NotEmpty();
            RuleFor(x => x.IsPublic).NotNull();
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

            var user = await _context.Set<UserProfile>().FirstOrDefaultAsync(u => u.UserId == userId);

            var e = new Event(
                newEventId,
                userId,
                request.EventName,
                request.Description,
                request.MaxAmountOfPeople,
                request.Place,
                new Point(request.Latitude, request.Longitude) { SRID = 4326 },
                request.StartDate,
                request.EndDate,
                request.Cost,
                request.MinAge,
                request.IsPublic,
                eCategory);

            e.AddParticipant(user);
            e.DomainEvents.Add(new EventCreatedEvent(newEventId, userId, user.Username));

            await _context.AddAsync(e, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return new AddEventResponse(
                e.Id,
                e.EventName,
                e.EventDescription,
                e.TakenPlacesAmount,
                e.MaxAmountOfPeople,
                e.Place,
                e.StartDate,
                e.Cost,
                e.MinAge,
                e.EventCategory.CategoryName,
                e.Participants.First(x => x.UserId == userId).Username
                );
        }
    }
}

public class EventCreatedEvent : DomainEvent
{
    public Guid EventId { get; }
    public Guid CreatorId { get; }
    public string CreatorName { get; }

    public EventCreatedEvent(Guid eventId, Guid creatorId, string creatorName)
    {
        EventId = eventId;
        CreatorId = creatorId;
        CreatorName = creatorName;
    }
}