using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions;

internal class FriendshipRequestDoesNotExistException : NotFoundException
{
    public FriendshipRequestDoesNotExistException() : base("Could not find friendship request.") { }
}
