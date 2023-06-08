using ComUnity.Application.Features.UserProfileManagement.Dtos;
using ComUnity.Application.Features.UserProfileManagement.Entities;

namespace ComUnity.Application.Features.ManagingEvents.Dtos;

public record EventDto(
    Guid Id,
    string OwnerName,
    string? OwnerPicture,
    string Name,
    int TakenPlaces,
    int MaxAmountOfPeople,
    string Place,
    DateTime EventDate,
    double Cost,
    int MinAge,
    string EventCategory,
    string? EventCategoryPicture,
    IEnumerable<UserDto> Participants);
