using CDF_Core.Entities.SnowFlake;
using CDF_Services.IServices.ISnowFlakeService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CDF_WebApi.Controllers.SnowFlake
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class SnowFlakeController : Controller
    {
        private ISnowFlakeService _snowFlakeService;
        public SnowFlakeController(ISnowFlakeService snowFlakeService) 
        {
            _snowFlakeService = snowFlakeService;
        }

        [HttpGet]
        [Route("getEmployeeData")]
        public async Task<IActionResult> getEmployeeData()
        {
            return await _snowFlakeService.ListEmployeeDetail();
        }
    }
}
