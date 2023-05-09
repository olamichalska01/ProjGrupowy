namespace ComUnity.Application.Common.Exceptions;

public class InvalidVerificationCodeException : BusinessRuleException
{
    public InvalidVerificationCodeException() : base("Verification code is invalid or expired.") { }
}
