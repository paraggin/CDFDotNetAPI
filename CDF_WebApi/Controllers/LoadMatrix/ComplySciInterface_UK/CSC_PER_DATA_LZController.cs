using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;
using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;

namespace CDF_WebApi.Controllers.LoadMatrix.ComplySciInterface_UK
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadMatrix_CSC_PER_DATA_LZController : ControllerBase
    {
        private ICSC_PER_DATA_LZService _iCSC_PER_DATA_LZService;

        public LoadMatrix_CSC_PER_DATA_LZController(ICSC_PER_DATA_LZService iCSC_PER_DATA_LZService)
        {
            _iCSC_PER_DATA_LZService = iCSC_PER_DATA_LZService;
        }

        [HttpGet]
        [Route("GetCSC_PER_DATA_LZData")]
        public CSC_PER_DATA_LZ_Response GetCSC_PER_DATA_LZData()
        {
            return _iCSC_PER_DATA_LZService.getCSC_PER_DATA_LZData();

        }
    }
}
