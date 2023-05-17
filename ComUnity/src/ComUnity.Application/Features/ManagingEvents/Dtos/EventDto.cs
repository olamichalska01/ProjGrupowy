namespace ComUnity.Application.Features.ManagingEvents.Dtos;

public record EventDto(
    Guid Id,
    string Name,
    int MaxAmountOfPeople,
    string Place,
    DateTime EventDate,
    double Cost,
    int MinAge,
    string EventCategory);
