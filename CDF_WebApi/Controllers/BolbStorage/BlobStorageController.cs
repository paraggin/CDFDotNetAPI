using Azure.Storage.Blobs;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Constants = CDF_Services.Constants.Constants;

namespace CDF_WebApi.Controllers.BolbStorage
{

    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class BlobStorageController : Controller
    {
        private readonly IBlobStorageService _BlobStorageService;
        private readonly Constants _IConstants;
        private readonly IConfiguration _configuration;
       
        public BlobStorageController(IBlobStorageService BlobStorageService, IConfiguration configuration, Constants IConstants)
        {
            _BlobStorageService = BlobStorageService;
            _configuration = configuration;
            _IConstants = IConstants;
           
        }       


        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string prefix)
        {
            BlobClient blobClient = new BlobClient(_configuration["AzureBlobStorage:ConnectionString"], _configuration["AzureBlobStorage:ContainerName"], prefix);
            using (var stream = new MemoryStream())
            {
               var temp= await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
                return File(stream.ToArray(), contentType, blobClient.Name);
            }
        }

        [HttpGet]
        [Route("getBLobSAS")]
        public async Task<IActionResult> getBLobSAS(string BlobName)
        {
           
           return await _BlobStorageService.getBLobSAS(BlobName);

        }

        [HttpGet]
        [Route("ListBlobsv2")]
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1, string? startdate = null, string? enddate = null, string? period = null, string? reportingUnit = null, string? filename = null)
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize,(int) pageNumber, startdate, enddate, period, reportingUnit, filename);

        }


    }
}
