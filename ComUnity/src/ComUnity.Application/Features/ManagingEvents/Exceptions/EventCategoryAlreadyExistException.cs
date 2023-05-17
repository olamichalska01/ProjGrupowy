using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.ManagingEvents.Exceptions;

public class EventCategoryAlreadyExistException : BusinessRuleException
{
    public EventCategoryAlreadyExistException() : base($"This category already exists.") { }
}
