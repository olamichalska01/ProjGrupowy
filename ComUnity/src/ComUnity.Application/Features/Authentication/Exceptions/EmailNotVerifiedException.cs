using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.Authentication.Exceptions;

public class EmailNotVerifiedException : BusinessRuleException
{
    public EmailNotVerifiedException() : base("Your account is inactive due to unverified email address.") { }
}
