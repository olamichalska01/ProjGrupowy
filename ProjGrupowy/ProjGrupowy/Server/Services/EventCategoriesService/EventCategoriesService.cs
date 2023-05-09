using Microsoft.EntityFrameworkCore;
using ProjGrupowy.Server.Models;
using ProjGrupowy.Shared;
using System.Runtime.InteropServices;

namespace ProjGrupowy.Server.Services.EventCategoriesService
{
    public class EventCategoriesService : IEventCategoriesService
    {
        private readonly DatabaseContext databaseContext;
        public EventCategoriesService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<IEnumerable<EventCategory>> GetEventsCategories()
        {
            return await databaseContext.EventCategories.ToListAsync();
        }

        public async Task<ServiceResponse<EventCategory>> AddEventCategory(string name)
        {
            var eventCategory = await databaseContext.EventCategories.FirstOrDefaultAsync(e => e.CategoryName == name);

            if(eventCategory != null)
            {
                return new ServiceResponse<EventCategory> { Message = "This category already exists.", Success = false };
            }

            eventCategory = new EventCategory()
            {
                CategoryName = name,
            };

            await databaseContext.EventCategories.AddAsync(eventCategory);
            await databaseContext.SaveChangesAsync();

            return new ServiceResponse<EventCategory> { Data = eventCategory, Message = "Event category added." };
        }

        public async Task<ServiceResponse<EventCategory>> DeleteEventCategory(string name)
        {
            var e = await databaseContext.EventCategories.FirstOrDefaultAsync(e => e.CategoryName == name);

            if (e == null)
            {
                return new ServiceResponse<EventCategory> { Message = "Couldn't find this event category.", Success = false };
            }

            databaseContext.EventCategories.Remove(e);
            await databaseContext.SaveChangesAsync();

            return new ServiceResponse<EventCategory> { Data = e, Message = "Deleted." };
        }


    }
}
