using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.HolidayCalendar
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IITDepartmentService _iTDepartmentService;

        public DepartmentController(IITDepartmentService iTDepartmentService)
        {
            _iTDepartmentService = iTDepartmentService;
        }

        [HttpGet]
        [Route("getITDepartment")]
        public async Task<IActionResult> getITDepartment()
        {
            return await _iTDepartmentService.ListITDepartment();
        }
    }
}
