using ComUnity.Application.Features.UserProfileManagement.Entities;
using NetTopologySuite.Geometries;

namespace ComUnity.Application.Features.ManagingEvents.Entities;

public class Post
{
    public Guid Id { get; private set; }

    public Guid AuthorId { get; private set; }

    public virtual UserProfile Author { get; private set; }

    public string AuthorName { get; private set; }

    public DateTime PublishedDate { get; private set; }

    public string PostName { get; private set; }

    public string PostText { get; private set; }


#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Post() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public Post(
        Guid Id,
        string AuthorName,
        Guid AuthorId,
        string PostName,
        DateTime PublishedDate,
        string PostText)
    {
        this.Id = Id;
        this.AuthorName = AuthorName;
        this.AuthorId = AuthorId;
        this.PublishedDate = PublishedDate;
        this.PostName = PostName;
        this.PostText = PostText;
    }
}
