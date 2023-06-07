using ComUnity.Application.Features.ManagingEvents.Entities;

namespace ComUnity.Application.Features.UserProfileManagement.Entities;

public class UserProfile
{
    public Guid UserId { get; private set; }

    public string Username { get; private set; }

    public Guid? ProfilePicture { get; private set; }

    public string? AboutMe{ get; private set; }

    public string? City { get; private set; }

    public DateTime DateOfBirth { get; private set; }

    public virtual ICollection<UserFavoriteEventCategory> FavoriteCategories { get; set; }
    public ICollection<Event> UserEvents { get; set; } = new List<Event>();

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

    public void ChangeProfilePicture(Guid newPictureId)
    {
        ProfilePicture = newPictureId;
    }

    public void UpdateBasicProfileInformation(string aboutMe, string city)
    {
        AboutMe = aboutMe;
        City = city;
    }
}
