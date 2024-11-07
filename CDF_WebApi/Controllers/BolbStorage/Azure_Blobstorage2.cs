using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class Azure_Blobstorage2 : ControllerBase
    {
        private readonly IAzure_BlobstorageService2 _BlobStorageService;

        public Azure_Blobstorage2(IAzure_BlobstorageService2 BlobStorageService)
        {
            _BlobStorageService = BlobStorageService;
        }


        [HttpPost]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-dev", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-dev")]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadBlob(file);

        }

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-dev", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-dev")]
        [Route("downloadBlob_Identity")]
        public async Task<IActionResult> downloadBlob_Identity(string prefix)
        {
            return await _BlobStorageService.downloadBlob(prefix);

        }

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-dev", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-dev")]
        [Route("deleteBlob")]
        public async Task<IActionResult> deleteBlob(string blobName)
        {
            return await _BlobStorageService.DeleteBlob(blobName);

        }

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-dev", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-dev")]
        [Route("ListBlobs_Identity")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1)
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize, (int)pageNumber);

        }


        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-dev", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-dev")]
        [Route("getJsonFromBlob")]
        public async Task<IActionResult> getJsonFromBlob(string BlobName)
        {
            return Ok(await _BlobStorageService.ConvertToJsonFromUrl(BlobName));

        }
    }
}
