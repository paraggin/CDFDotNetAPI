using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Services.IServices.IBlobStorageService;
using Swashbuckle.AspNetCore.Annotations;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController_StefanoController : ControllerBase
    {
        private readonly IBlobStorageService_Stefano _BlobStorageService;
       
        public BlobController_StefanoController(IBlobStorageService_Stefano BlobStorageService) {
            _BlobStorageService = BlobStorageService;   
        }

        [HttpPost]
        [SwaggerOperation(Summary = "AC-CON", Description = "TEST Description")]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadBlob(file);

        }

        [HttpPost]
        [Route("downloadBlob_Identity")]
        public async Task<IActionResult> downloadBlob_Identity(string prefix)
        {
            return await _BlobStorageService.downloadBlob(prefix);

        }

        [HttpGet]
        [Route("ListBlobs_Identity")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1)
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize, (int)pageNumber);

        }


        [HttpGet]
        [Route("getJsonFromBlob")]
        public async Task<IActionResult> getJsonFromBlob(string BlobName)
        {
            return Ok(await _BlobStorageService.ConvertToJsonFromUrl(BlobName));

        }
    }
}
