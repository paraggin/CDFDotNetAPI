using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using CDF_Services.IServices.IBlobStorageService;

namespace CDF_WebApi.Controllers.BolbStorage
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class Azure_Blobstorage : ControllerBase
    {
        private readonly IAzure_BlobstorageService _BlobStorageService;

        public Azure_Blobstorage(IAzure_BlobstorageService BlobStorageService)
        {
            _BlobStorageService = BlobStorageService;
        }


        [HttpPost]
        [SwaggerOperation(Summary = "lzdeu1corpetldev01 :  sftp-esker", Description = "AccountName : lzdeu1corpetldev01 ,  ContainerName : sftp-esker ")]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadDynamicBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadBlob(file);

        }
        /*
                [HttpGet]
                [SwaggerOperation(Summary = "lzdeu1corpetldev01 :  sftp-esker", Description = "AccountName : lzdeu1corpetldev01 ,  ContainerName : sftp-esker ")]
                [Route("downloadBlob_Identity")]
                public async Task<IActionResult> downloadBlob_Identity(string prefix)
                {
                    return await _BlobStorageService.downloadBlob(prefix);

                }

                [HttpGet]
                [SwaggerOperation(Summary = "lzdeu1corpetldev01 :  sftp-esker", Description = "AccountName : lzdeu1corpetldev01 ,  ContainerName : sftp-esker ")]
                [Route("deleteBlob")]
                public async Task<IActionResult> deleteBlob(string blobName)
                {
                    return await _BlobStorageService.DeleteBlob(blobName);

                }*/

        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev01 :  sftp-esker", Description = "AccountName : lzdeu1corpetldev01 ,  ContainerName : sftp-esker ")]
        [Route("ListBlobs_Identity")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1)
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize, (int)pageNumber);

        }


        [HttpGet]
        [SwaggerOperation(Summary = "lzdeu1corpetldev01 :  sftp-esker", Description = "AccountName : lzdeu1corpetldev01 ,  ContainerName : sftp-esker ")]
        [Route("getJsonFromBlob")]
        public async Task<IActionResult> getJsonFromBlob(string BlobName)
        {
            return Ok(await _BlobStorageService.ConvertToJsonFromUrl(BlobName));

        }
    }
}
