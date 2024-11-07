using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class Azure_Blobstorage3 : ControllerBase
    {
        private readonly IAzure_BlobstorageService3 _BlobStorageService;

        public Azure_Blobstorage3(IAzure_BlobstorageService3 BlobStorageService)
        {
            _BlobStorageService = BlobStorageService;
        }


        [HttpPost]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-test", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-test")]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadBlob(file);

        }

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-test", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-test")]
        [Route("downloadBlob_Identity")]
        public async Task<IActionResult> downloadBlob_Identity(string prefix)
        {
            return await _BlobStorageService.downloadBlob(prefix);

        }

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-test", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-test")]
        [Route("deleteBlob")]
        public async Task<IActionResult> deleteBlob(string blobName)
        {
            return await _BlobStorageService.DeleteBlob(blobName);

        }

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-test", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-test")]
        [Route("ListBlobs_Identity")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1)
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize, (int)pageNumber);

        }


        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev02 :  alteryx-test", Description = "AccountName : lzdeu1corpetldev02 ,  ContainerName : alteryx-test")]
        [Route("getJsonFromBlob")]
        public async Task<IActionResult> getJsonFromBlob(string BlobName)
        {
            return Ok(await _BlobStorageService.ConvertToJsonFromUrl(BlobName));

        }
    }
}
