using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.HolidayCalendar
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class WorkForceMasterController : ControllerBase
    {

        private readonly IWorkForceMasterService _WorkForceMasterServices;

        public WorkForceMasterController(IWorkForceMasterService WorkForceMasterServices)
        {
            _WorkForceMasterServices = WorkForceMasterServices;
        }
        [HttpGet]
        [Route("getWorkForceMaster")]
        public async Task<IActionResult> getWorkForceMaster()
        {
            return await _WorkForceMasterServices.ListWorkForceMaster();
        }
    }
}
