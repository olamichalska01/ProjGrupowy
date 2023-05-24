using ComUnity.Application.Features.ManagingEvents.Entities;

namespace ComUnity.Application.Features.UserProfileManagement.Entities;

public class UserFavoriteEventCategory
{
    public Guid UserId { get; set; }
    public Guid EventCategoryId { get; set; }
    public virtual UserProfile User { get; set; } // navigation property
    public virtual EventCategory Category { get; set; } // navigation property

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UserFavoriteEventCategory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public UserFavoriteEventCategory(Guid userId, Guid eventCategoryId)
    {
        UserId = userId;
        EventCategoryId = eventCategoryId;
    }
}

