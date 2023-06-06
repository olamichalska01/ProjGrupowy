using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions;

internal class FriendshipArleadyExistsException : BusinessRuleException
{
    public FriendshipArleadyExistsException() : base("Friendship already exists.") { }
}
