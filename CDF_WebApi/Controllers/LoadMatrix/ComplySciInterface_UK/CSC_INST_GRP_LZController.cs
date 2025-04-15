using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;
using CDF_Services.Services.LoadMatric.ComplySciInterface_UK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.LoadMatrix.ComplySciInterface_UK
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadMatrix_CSC_INST_GRP_LZController : ControllerBase
    {

        private ICSC_INST_GRP_LZService _iNST_GRP_LZService;

        public LoadMatrix_CSC_INST_GRP_LZController(ICSC_INST_GRP_LZService iNST_GRP_LZService)
        {
            _iNST_GRP_LZService = iNST_GRP_LZService;
        }

        [HttpGet]
        [Route("getCSC_INST_GRP_LZData")]
        public CSC_INST_GRP_LZResponse getCSC_INST_GRP_LZData()
        {
            return _iNST_GRP_LZService.getCSC_INST_GRP_LZData();

        }

    }
}
