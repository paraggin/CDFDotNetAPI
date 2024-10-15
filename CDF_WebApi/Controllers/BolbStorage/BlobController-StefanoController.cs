using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CDF_Services.IServices.IBlobStorageService;
using Swashbuckle.AspNetCore.Annotations;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobControllerController : ControllerBase
    {
        private readonly IBlobStorageService_Stefano _BlobStorageService;
       
        public BlobControllerController(IBlobStorageService_Stefano BlobStorageService) {
            _BlobStorageService = BlobStorageService;   
        }

        /*[HttpPost]
        [SwaggerOperation(Summary = "AC-CON", Description = "TEST Description")]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadBlob(file);

        }

        [HttpGet]
        [Route("downloadBlob_Identity")]
        public async Task<IActionResult> downloadBlob_Identity(string name)
        {
            return await _BlobStorageService.downloadBlob(name);

        }

        [HttpGet]
        [Route("deleteBlob")]
        public async Task<IActionResult> deleteBlob(string blobName)
        {
            return await _BlobStorageService.DeleteBlob(blobName);

        }*/

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
