using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.HolidayCalendar
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayCalendarController : ControllerBase
    {

        private readonly IHolidayCalendarService _HolidayServices;     

        public HolidayCalendarController(IHolidayCalendarService HolidayServices)
        {
            _HolidayServices = HolidayServices;         
        }
        [HttpGet]
        [Route("getCalendarEvents")]
        public async Task<IActionResult> getCalendarEvents()
        {
            return await _HolidayServices.ListEvents();
        }
        
    }
}
