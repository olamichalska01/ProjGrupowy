using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjGrupowy.Server.Services.EventsService;
using ProjGrupowy.Shared;
using System.Collections;

namespace ProjGrupowy.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventsService eventsService;

        public EventsController(IEventsService eventsService)
        {
            this.eventsService = eventsService;
        }

        [HttpGet("GetEvents")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return Ok(await eventsService.GetEvents());
        }

        [HttpGet("GetEventById/{id:int}")]
        public async Task<ActionResult<ServiceResponse<Event>>> GetEventById(int id)
        {
            var response = await eventsService.GetEventById(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("AddEvent")]
        public async Task<ActionResult<IEnumerable<Event>>> AddEvent(EventDto eDto)
        {
            var response = await eventsService.AddEvent(eDto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpDelete("DeleteEvent/{id:int}")]
        public async Task<ActionResult<IEnumerable<Event>>> DeleteEvent(int id)
        {
            var response = await eventsService.DeleteEvent(id);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
