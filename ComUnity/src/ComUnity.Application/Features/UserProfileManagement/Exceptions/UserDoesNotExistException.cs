using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.UserProfileManagement.Exceptions;

internal class UserDoesNotExistException : NotFoundException
{
    public UserDoesNotExistException(Guid userId) : base($"User with id {userId} does not exist.") { }
}