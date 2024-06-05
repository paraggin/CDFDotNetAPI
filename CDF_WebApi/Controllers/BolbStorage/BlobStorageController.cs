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
        [Route("uploadBlobTest")]
        public async Task<IActionResult> uploadBlobTest()
        {
            return await _BlobStorageService.uploadBlobTest();

        }

        [HttpPost]
        [Route("webhook")]
        public void ReceiveWebhook([FromBody] dynamic payload)
        {
           _BlobStorageService.ReceiveWebhook(payload);

        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string prefix)
        {
            return await _BlobStorageService.DownloadFile(prefix);                
        
        }

        [HttpGet]
        [Route("getBLobSAS")]
        public async Task<IActionResult> getBLobSAS(string BlobName)
        {
           
           return await _BlobStorageService.getBLobSAS(BlobName);

        }

        [HttpGet]
        [Route("ListBlobsv2")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1,  string? period = null, string? reportingUnit = null, string? filename = null,string? containerName= "")
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize,(int) pageNumber, period, reportingUnit, filename, containerName);

        }

        [HttpGet]
        [Route("ListBlobsv3")]
        public async Task<IActionResult> ListBlobsv3(int? pageSize = 10)
        {
            return await _BlobStorageService.ListBlobsAsyncREST();

        }

        [HttpPost]
        [Route("uploadBlob")]
        public async Task<IActionResult> uploadBlob(IFormFile file, string? containerName = "container-poc")
        {
            return await _BlobStorageService.uploadBlob(file, containerName);   
        }

        [HttpPost]
        [Route("uploadBlobMultiple")]
        public async Task<IActionResult> uploadBlobMultiple(IFormFile file, int numberOfUploads = 10000, string? containerName = "container-poc")
        {
            return    await _BlobStorageService.uploadBlobMultiple(file, numberOfUploads, containerName); 
        }         


    }
}

    

