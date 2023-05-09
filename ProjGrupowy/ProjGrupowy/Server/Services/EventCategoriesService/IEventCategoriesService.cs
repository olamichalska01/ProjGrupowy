using ProjGrupowy.Server.Models;
using ProjGrupowy.Shared;

namespace ProjGrupowy.Server.Services.EventCategoriesService
{
    public interface IEventCategoriesService
    {
        Task<IEnumerable<EventCategory>> GetEventsCategories();

        Task<ServiceResponse<EventCategory>> AddEventCategory(string name);

        Task<ServiceResponse<EventCategory>> DeleteEventCategory(string name);
    }
}
