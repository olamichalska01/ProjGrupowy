using Microsoft.EntityFrameworkCore;
using ProjGrupowy.Server.Models;
using ProjGrupowy.Shared;
using System.Runtime.InteropServices;

namespace ProjGrupowy.Server.Services.EventsService
{
    public class EventsService : IEventsService
    {
        private readonly DatabaseContext databaseContext;
        public EventsService(DatabaseContext databaseContext)
        {
            this.databaseContext = databaseContext;
        }

        public async Task<IEnumerable<Event>> GetEvents()
        {
            return await databaseContext.Events.Include(e => e.EventCategory).ToListAsync();
        }

        public async Task<ServiceResponse<Event>> GetEventById(int id)
        {
            var e = await databaseContext.Events.Include(e => e.EventCategory).FirstOrDefaultAsync(e => e.Id == id);

            if (e == null)
            {
                return new ServiceResponse<Event> { Message = "Couldn't find this event.", Success = false };
            }

            return new ServiceResponse<Event> { Data = e, Message = "Event found." };
        }

        public async Task<ServiceResponse<Event>> AddEvent(EventDto eDto)
        {
            var eCategory = await databaseContext.EventCategories.FirstOrDefaultAsync(ec => ec.CategoryName == eDto.EventCategory);

            if (eCategory == null)
            {
                return new ServiceResponse<Event> { Message = "Supplied event category doesn't exist.", Success = false };
            }

            var e = new Event()
            {
                EventName = eDto.EventName,
                MaxAmountOfPeople = eDto.MaxAmountOfPeople,
                Place = eDto.Place,
                EventDate = eDto.EventDate,
                Cost = eDto.Cost,
                MinAge = eDto.MinAge,
                EventCategory = eCategory
            };

            await databaseContext.Events.AddAsync(e);
            await databaseContext.SaveChangesAsync();

            return new ServiceResponse<Event> { Data = e, Message = "Event added." };
        }

        public async Task<ServiceResponse<Event>> DeleteEvent(int id)
        {
            var e = await databaseContext.Events.FirstOrDefaultAsync(e => e.Id == id);

            if (e == null)
            {
                return new ServiceResponse<Event> { Message = "Couldn't find this event.", Success = false };
            }

            databaseContext.Events.Remove(e);
            await databaseContext.SaveChangesAsync();

            return new ServiceResponse<Event> { Data = e, Message = "Deleted." };
        }


    }
}
