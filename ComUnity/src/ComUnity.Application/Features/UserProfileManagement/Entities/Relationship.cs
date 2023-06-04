using ComUnity.Application.Common;

namespace ComUnity.Application.Features.UserProfileManagement.Entities;

public class Relationship : IHasDomainEvent
{
    public Guid Id { get; private set; }

    public Guid User1Id { get; private set; }

    public UserProfile User1 { get; private set; }

    public Guid User2Id { get; private set; }

    public UserProfile User2 { get; private set; }

    public string RelationshipType { get; private set; }

    public List<DomainEvent> DomainEvents { get; } = new List<DomainEvent>();

    public Relationship() { }

    public Relationship(Guid id, Guid user1, Guid user2, string relationshipType)
    {
        Id = id;
        User1Id = user1;
        User2Id = user2;
        RelationshipType = relationshipType;
    }

    public void AcceptFriendship()
    {
        RelationshipType = RelationshipTypes.Friendship;
        DomainEvents.Add(new FriendshipRequestAccepted(User1Id, User2Id, User2.Username));
    }
}