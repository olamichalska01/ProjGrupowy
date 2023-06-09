using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions;

internal class FriendshipDoesNotExistsException : BusinessRuleException
{
    public FriendshipDoesNotExistsException() : base("Friendship already exists.") { }
}
