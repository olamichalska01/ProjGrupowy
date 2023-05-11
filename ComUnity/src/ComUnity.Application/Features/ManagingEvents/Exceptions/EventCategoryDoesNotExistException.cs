using ComUnity.Application.Common.Exceptions;

namespace ComUnity.Application.Features.ManagingEvents.Exceptions;

public class EventCategoryDoesNotExistException : BusinessRuleException
{
    public EventCategoryDoesNotExistException(string eventCategoryName) : base($"Supplied event category {eventCategoryName} doesn't exist.") { }
}
