using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.LoadMatrix.ComplySciInterface_UK
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadMatrix_UK_PER_CFTY_LZ_Controller : ControllerBase
    {
        private IUK_PER_CFTY_LZService _PER_CFTY_LZService;

        public LoadMatrix_UK_PER_CFTY_LZ_Controller(IUK_PER_CFTY_LZService pER_CFTY_LZService)
        {
            _PER_CFTY_LZService = pER_CFTY_LZService;
        }

        [HttpGet]
        [Route("getUK_PER_CFTY_LZData")]
        public UK_PER_CFTY_LZ_Response getUK_PER_CFTY_LZData()
        {
            return _PER_CFTY_LZService.getUK_PER_CFTY_LZData();

        }
    }
}
