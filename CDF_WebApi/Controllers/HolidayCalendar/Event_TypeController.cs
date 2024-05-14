using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.HolidayCalendar
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class Event_TypeController : ControllerBase
    {
        private readonly IEvent_TypeService _Event_TypeServices;

        public Event_TypeController(IEvent_TypeService Event_TypeServices)
        {
            _Event_TypeServices = Event_TypeServices;
        }
        [HttpGet]
        [Route("getEventType")]
        public async Task<IActionResult> getEventType()
        {
            return await _Event_TypeServices.ListEvent_TypeList();
        }
    }
}
