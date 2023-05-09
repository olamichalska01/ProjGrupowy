using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.Authentication.Exceptions;

internal class UserAlreadyExistsException : BusinessRuleException
{
    public UserAlreadyExistsException() : base("User with this email already exists.") { }
}
