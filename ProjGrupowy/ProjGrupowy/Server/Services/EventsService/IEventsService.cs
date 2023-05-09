using ProjGrupowy.Server.Models;
using ProjGrupowy.Shared;

namespace ProjGrupowy.Server.Services.EventsService
{
    public interface IEventsService
    {
        Task<IEnumerable<Event>> GetEvents();

        Task<ServiceResponse<Event>> GetEventById(int id);

        Task<ServiceResponse<Event>> AddEvent(EventDto eDto);

        Task<ServiceResponse<Event>> DeleteEvent(int id);
    }
}
