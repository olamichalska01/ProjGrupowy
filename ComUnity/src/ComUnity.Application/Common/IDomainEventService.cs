namespace ComUnity.Application.Common;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
