using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CDF_WebApi.Controllers.HolidayCalendar
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _HolidayServices;
        private readonly IConfiguration _configuration;

        public HolidayController(IHolidayService HolidayServices, IConfiguration configuration)
        {
            _HolidayServices = HolidayServices;
            _configuration = configuration;
        }
       

        [HttpGet]
        [Route("getCalendarHolidays")]
        public async Task<IActionResult> getCalendarHolidays()
        {
            return await _HolidayServices.ListHolidays();
        }
    }
}
