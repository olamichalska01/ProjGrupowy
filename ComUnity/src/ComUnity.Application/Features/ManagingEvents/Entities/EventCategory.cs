using ComUnity.Application.Features.UserProfileManagement.Entities;

namespace ComUnity.Application.Features.ManagingEvents.Entities;

public class EventCategory
{
    private string name;

    public Guid EventCategoryId { get; private set;  }
    public string CategoryName { get; private set; }
    public virtual ICollection<Event>? Events { get; private set; }
    public virtual ICollection<UserProfile> Users { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public EventCategory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public EventCategory(string categoryName, Guid eventCategoryId) 
    {
        CategoryName = categoryName;
        EventCategoryId = eventCategoryId;
    }

    public EventCategory(string name)
    {
        this.name = name;
    }
}