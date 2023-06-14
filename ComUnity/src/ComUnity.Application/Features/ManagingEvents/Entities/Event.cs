using ComUnity.Application.Features.UserProfileManagement.Entities;
using NetTopologySuite.Geometries;

namespace ComUnity.Application.Features.ManagingEvents.Entities;

public class Event
{
    public Guid Id { get; private set; }

    public virtual UserProfile Owner { get; private set; }

    public Guid OwnerId { get; private set; }

    public string EventName { get; private set; }

    public int MaxAmountOfPeople { get; private set; }

    public int TakenPlacesAmount { get; private set; }

    public string Place { get; private set; }

    public Point Location { get; private set; }

    public DateTime StartDate { get; private set; }

    public DateTime EndDate { get; private set; }

    public double Cost { get; private set; }

    public int MinAge { get; private set; }

    public bool IsPublic { get; private set; }

    public EventCategory EventCategory { get; private set; }

    public ICollection<UserProfile> Participants { get; private set; } = new List<UserProfile>();

    public ICollection<Post> Posts { get; private set; } = new List<Post>();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Event() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Event(
        Guid id,
        Guid ownerId,
        string eventName,
        int maxAmountOfPeople,
        string place,
        Point location,
        DateTime startDate,
        DateTime endDate,
        double cost,
        int minAge,
        bool isPublic,
        EventCategory eventCategory
        )
    {
        Id = id;
        OwnerId = ownerId;
        EventName = eventName;
        TakenPlacesAmount = 0;
        MaxAmountOfPeople = maxAmountOfPeople;
        Place = place;
        Location = location;
        StartDate = startDate;
        EndDate = endDate;
        Cost = cost;
        MinAge = minAge;
        IsPublic = isPublic;
        EventCategory = eventCategory;
    }

    public void AddParticipant(UserProfile newParticipant)
    {
        Participants.Add( newParticipant );
        TakenPlacesAmount++;
    }

    public void RemoveParticipant(UserProfile participant)
    {
        Participants.Remove(participant);
        TakenPlacesAmount--;
    }

    public void AddPost(Post newPost)
    {
        Posts.Add(newPost);
    }
}
