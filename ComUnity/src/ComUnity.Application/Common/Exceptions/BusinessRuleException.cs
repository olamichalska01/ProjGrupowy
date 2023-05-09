using System.Runtime.Serialization;

namespace ComUnity.Application.Common.Exceptions;

public class BusinessRuleException : Exception
{
    public BusinessRuleException() : base() { }

    public BusinessRuleException(string? message) : base(message)
    {
    }

    public BusinessRuleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected BusinessRuleException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
