using ComUnity.Application.Features.ManagingEvents.Entities;

namespace ComUnity.Application.Features.UserProfileManagement.Entities;

public class UserProfile
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public virtual ICollection<EventCategory> FavoriteCategories { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UserProfile() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public UserProfile(Guid userId, string username)
    {
        UserId = userId;
        Username = username;
    }
}
