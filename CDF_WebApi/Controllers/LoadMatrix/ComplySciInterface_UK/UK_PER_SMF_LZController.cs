using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;
using CDF_Services.Services.LoadMatric.ComplySciInterface_UK;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.LoadMatrix.ComplySciInterface_UK
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class LoadMatrix_UK_PER_SMF_LZController : ControllerBase
    {
        private IUK_PER_SMF_LZService _uk_PER_SMF_LZService;

        public LoadMatrix_UK_PER_SMF_LZController(IUK_PER_SMF_LZService uk_PER_SMF_LZService)
        {
            _uk_PER_SMF_LZService = uk_PER_SMF_LZService;
        }

        [HttpGet]
        [Route("getUK_PER_SMF_LZData")]
        public UK_PER_SMF_LZ_Response getUK_PER_SMF_LZData()
        {
            return _uk_PER_SMF_LZService.getUK_PER_SMF_LZData();

        }
    }
}
