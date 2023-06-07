using ComUnity.Application.Features.UserProfileManagement.Entities;
using NetTopologySuite.Geometries;

namespace ComUnity.Application.Features.ManagingEvents.Entities;

public class Event
{
    public Guid Id { get; private set; }

    public Guid OwnerId { get; private set; }

    public string EventName { get; private set; }

    public int MaxAmountOfPeople { get; private set; }

    public int TakenPlacesAmount { get; set; }

    public string Place { get; private set; }

    public Point Location { get; private set; }

    public DateTime EventDate { get; private set; }

    public double Cost { get; private set; }

    public int MinAge { get; private set; }

    public bool IsPublic { get; private set; }

    public EventCategory EventCategory { get; private set; }

    public ICollection<UserProfile> Participants { get; private set; } = new List<UserProfile>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Event() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Event(
        Guid id,
        Guid ownerId,
        string eventName,
        int takenPlacesAmount,
        int maxAmountOfPeople,
        string place,
        Point location,
        DateTime eventDate,
        double cost,
        int minAge,
        EventCategory eventCategory,
        ICollection<UserProfile> participants
        )
    {
        Id = id;
        OwnerId = ownerId;
        EventName = eventName;
        TakenPlacesAmount = takenPlacesAmount;
        MaxAmountOfPeople = maxAmountOfPeople;
        Place = place;
        Location = location;
        EventDate = eventDate;
        Cost = cost;
        MinAge = minAge;
        EventCategory = eventCategory;
        Participants = participants;
    }
}
