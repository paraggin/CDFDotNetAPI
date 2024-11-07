using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDF_WebApi.Controllers.BolbStorage
{

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : Controller
    {
        private readonly IBlobStorageService _BlobStorageService;

        public BlobStorageController(IBlobStorageService BlobStorageService)
        {
            _BlobStorageService = BlobStorageService;
        }

        [HttpPost]
        [Route("uploadBlob_Identity")]
        public async Task<IActionResult> uploadBlob_Identity(IFormFile file)
        {
            return await _BlobStorageService.uploadDynamicBlobTest(file);

        }

        [HttpPost]
        [Route("downloadBlob_Identity")]
        public async Task<IActionResult> downloadBlob_Identity(string prefix)
        {
            return await _BlobStorageService.downloadBlobTest(prefix);

        }

        [HttpGet]
        [Route("deleteBlob")]
        public async Task<IActionResult> deleteBlob(string blobName)
        {
            return await _BlobStorageService.DeleteBlob(blobName);

        }

        [HttpGet]
        [Route("ListBlobs_Identity")]
        public async Task<IActionResult> ListBlobs_Identity(int? pageSize = 10, int? pageNumber = 1, string? period = null, string? reportingUnit = null, string? filename = null, string? containerName = "")
        {
            return await _BlobStorageService.ListBlobs_Identity((int)pageSize, (int)pageNumber, period, reportingUnit, filename, containerName);

        }

        [HttpGet]
        [Route("ListBlobsv2")]
        public async Task<IActionResult> ListBlobsv2(int? pageSize = 10, int? pageNumber = 1, string? period = null, string? reportingUnit = null, string? filename = null, string? containerName = "")
        {
            return await _BlobStorageService.ListBlobs_Identity((int)pageSize, (int)pageNumber, period, reportingUnit, filename, containerName);

        }

        //[HttpGet]
        //[Route("GetVersion")]
        //public IActionResult GetVersion()
        //{
        //    string response = "Version : 0.2";
        //    return Ok(response);
        //}

        [HttpGet]
        [Route("getBLobSAS_Identity")]
        public async Task<IActionResult> getBLobSASIdentity(string BlobName)
        {

            return await _BlobStorageService.getBLobSASIdentity(BlobName);

        }

        [HttpGet]
        [Route("getBLobSasUrl_Dynamic")]
        public async Task<IActionResult> getBLobSasUrl(string storageAccountName, string containerName, string blobName)
        {

            return await _BlobStorageService.getBlobSasUrl_Dynamic(storageAccountName, containerName, blobName);

        }

        [HttpGet]
        [Route("getJsonFromBlob")]
        public async Task<IActionResult> getJsonFromBlob(string BlobName)
        {
            return Ok(await _BlobStorageService.ConvertToJsonFromUrl(BlobName));

        }
        /*
                [HttpPost]
                [Route("webhook")]
                public void ReceiveWebhook([FromBody] dynamic payload)
                {
                    _BlobStorageService.ReceiveWebhook(payload);

                }*/

        /* [HttpGet]
         [Route("DownloadFile")]
         public async Task<IActionResult> DownloadFile(string prefix)
         {
             return await _BlobStorageService.DownloadFile(prefix);

         }*/



        /*  [HttpGet]
          [Route("getBLobSAS")]
          public async Task<IActionResult> getBLobSAS(string BlobName)
          {

              return await _BlobStorageService.getBLobSAS(BlobName);

          }*/


        /* [HttpGet]
         [Route("ListBlobsv2")]
         public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1, string? period = null, string? reportingUnit = null, string? filename = null, string? containerName = "")
         {
             return await _BlobStorageService.FilterBlobs((int)pageSize, (int)pageNumber, period, reportingUnit, filename, containerName);

         }*/




        /*   [HttpPost]
           [Route("uploadBlob")]
           public async Task<IActionResult> uploadBlob(IFormFile file, string? containerName = "blob-upload-cdf")
           {
               return await _BlobStorageService.uploadBlob(file, containerName);
           }*/

        /* [HttpPost]
         [Route("uploadBlobMultiple")]
         public async Task<IActionResult> uploadBlobMultiple(IFormFile file, int numberOfUploads = 10000, string? containerName = "blob-upload-cdf")
         {
             return await _BlobStorageService.uploadBlobMultiple(file, numberOfUploads, containerName);
         }*/


    }
}



