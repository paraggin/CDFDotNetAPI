using CDF_Services.IServices.LoadMatrix.BusinessContinuity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CDF_Services.IServices.IDocViewerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CDF_Core.Entities.LoadMatrix.BusinessContinuity;

namespace CDF_WebApi.Controllers.LoadMatrix
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessContinutyController : ControllerBase
    {
        private readonly IBusinessContinutyService _BusinessContinutyService;

        public BusinessContinutyController(IBusinessContinutyService businessContinutyService)
        {
            _BusinessContinutyService = businessContinutyService;
        }

        [HttpGet]
        [Route("GetCrisisManagementData")]
        public CrisisMgmtResponse GetCrisisManagementData()
        {

            return _BusinessContinutyService.GetCrisisManagementData();

        }

    }
}
