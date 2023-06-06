using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.Authentication.Exceptions;

public class UsernameTakenException : BusinessRuleException
{
    public UsernameTakenException() : base("Username is already taken.") { }
}
