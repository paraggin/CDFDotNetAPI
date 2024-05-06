using CDF_Core.Entities.PNP_Accounts;
using CDF_Services.IServices.IHolidayCalendarServices;
using CDF_Services.Services.HolidayCalendarServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Constants = CDF_Services.Constants.Constants;

namespace CDF_WebApi.Controllers.HolidayCalendar
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayCalendarController : ControllerBase
    {

        private readonly IHolidayCalendarService _HolidayServices;
        private readonly Constants _IConstants;
        private readonly IConfiguration _configuration;

        public HolidayCalendarController(IHolidayCalendarService HolidayServices, IConfiguration configuration, Constants IConstants)
        {
            _HolidayServices = HolidayServices;
            _configuration = configuration;
            _IConstants = IConstants;
        }
        [HttpGet]
        [Route("getCalendarEvents")]
        public async Task<IActionResult> getCalendarEvents()
        {
            return await _HolidayServices.ListEvents();
        }
    }
}
