using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.Authentication.Exceptions;

public class InvalidEmailOrPasswordException : BusinessRuleException
{
    public InvalidEmailOrPasswordException() : base("Invalid email or password") { }
}
