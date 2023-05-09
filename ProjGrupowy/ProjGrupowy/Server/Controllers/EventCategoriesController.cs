using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjGrupowy.Server.Services.EventCategoriesService;
using ProjGrupowy.Server.Services.EventsService;
using ProjGrupowy.Shared;

namespace ProjGrupowy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventCategoriesController : ControllerBase
    {
        private readonly IEventCategoriesService eventCategoriesService;

        public EventCategoriesController(IEventCategoriesService eventCategoriesService)
        {
            this.eventCategoriesService = eventCategoriesService;
        }

        [HttpGet("GetEventCategories")]
        public async Task<ActionResult<IEnumerable<EventCategory>>> GetEventCategories()
        {
            return Ok(await eventCategoriesService.GetEventsCategories());
        }

        [HttpPost("AddEventCategory")]
        public async Task<ActionResult<IEnumerable<Event>>> AddEventCategory(string name)
        {
            var response = await eventCategoriesService.AddEventCategory(name);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteEventCategory/{name}")]
        public async Task<ActionResult<IEnumerable<Event>>> DeleteEvent(string name)
        {
            var response = await eventCategoriesService.DeleteEventCategory(name);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }

}
