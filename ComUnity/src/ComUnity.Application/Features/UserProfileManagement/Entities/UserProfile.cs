using ComUnity.Application.Features.ManagingEvents.Entities;

namespace ComUnity.Application.Features.UserProfileManagement.Entities;

public class UserProfile
{
    public Guid UserId { get; private set; }

    public string Username { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public virtual ICollection<UserFavoriteEventCategory> FavoriteCategories { get; set; }

    public virtual ICollection<Relationship>? Relationships { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public UserProfile() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public UserProfile(Guid userId, string username, DateTime dateOfBirth)
    {
        UserId = userId;
        Username = username;
        DateOfBirth = dateOfBirth;
    }
}
