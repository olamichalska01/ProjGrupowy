namespace ComUnity.Application.Features.ManagingEvents.Entities;

public class EventCategory
{
    public string CategoryName { get; private set; }

    public virtual ICollection<Event>? Events { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EventCategory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public EventCategory(string categoryName) 
    {
        CategoryName = categoryName;
    }
}