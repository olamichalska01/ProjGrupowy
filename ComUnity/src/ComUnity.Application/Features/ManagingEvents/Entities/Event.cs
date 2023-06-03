namespace ComUnity.Application.Features.ManagingEvents.Entities;

public class Event
{
    public Guid Id { get; private set; }

    public string EventName { get; private set; }

    public int MaxAmountOfPeople { get; private set; }

    public int TakenPlacesAmount { get; private set; }

    public string Place { get; private set; }

    public DateTime EventDate { get; private set; }

    public double Cost { get; private set; }

    public int MinAge { get; private set; }

    public EventCategory EventCategory { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Event() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Event(
        Guid id,
        string eventName,
        int maxAmountOfPeople,
        string place,
        DateTime eventDate,
        double cost,
        int minAge,
        EventCategory eventCategory)
    {
        Id = id;
        EventName = eventName;
        MaxAmountOfPeople = maxAmountOfPeople;
        Place = place;
        EventDate = eventDate;
        Cost = cost;
        MinAge = minAge;
        EventCategory = eventCategory;
    }
}
