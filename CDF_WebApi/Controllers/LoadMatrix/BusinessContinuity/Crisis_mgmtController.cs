using CDF_Services.IServices.LoadMatrix.BusinessContinuity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Core.Entities.LoadMatrix.BusinessContinuity;

namespace CDF_WebApi.Controllers.LoadMatrix.BusinessContinuity
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]

    public class LoadMatrix_Crisis_mgmtController : ControllerBase
    {
        private readonly ICrisis_mgmtService _BusinessContinutyService;

        public LoadMatrix_Crisis_mgmtController(ICrisis_mgmtService businessContinutyService)
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
