using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.Authentication.Exceptions;

internal class UserAlreadyExists : BusinessRuleException
{
    public UserAlreadyExists() : base("User with this email already exists.") { }
}
