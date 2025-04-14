using CDF_Services.IServices.LoadMatrix.BusinessContinuity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Core.Entities.LoadMatrix.BusinessContinuity;

namespace CDF_WebApi.Controllers.LoadMatrix.BusinessContinuity
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadMatrix_Leadership_LZController : ControllerBase
    {
        private ILeadership_LZService _leadership_LZService;

        public LoadMatrix_Leadership_LZController(ILeadership_LZService leadership_LZService)
        {
            _leadership_LZService = leadership_LZService;
        }
        [HttpGet]
        [Route("GetLeadershipLZData")]
        public Leadership_LZResponse GetLeadershipLZData()
        {
            return _leadership_LZService.GetLeadershipLZData();

        }


    }
}
