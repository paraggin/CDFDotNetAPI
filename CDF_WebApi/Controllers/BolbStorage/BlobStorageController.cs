using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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
        public async Task<IActionResult> FilterBlobs(int? pageSize = 10, int? pageNumber = 1,  string? period = null, string? reportingUnit = null, string? filename = null,string? containerName= "")
        {
            return await _BlobStorageService.FilterBlobs((int)pageSize,(int) pageNumber, period, reportingUnit, filename, containerName);

        }

        [HttpPost]
        [Route("uploadBlob")]
        public async Task<IActionResult> uploadBlob(IFormFile file, string? containerName = "container-poc")
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    if (file.Length > 0)
                    {
                        string ConnectionString = _configuration["AzureBlobStorage:ConnectionString"];

                        var fileName = Path.GetFileName(file.FileName);
                        var fileType = Path.GetExtension(fileName);

                        BlobContainerClient blobContainerClient = new BlobContainerClient(ConnectionString, containerName);

                        if (!await blobContainerClient.ExistsAsync())
                        {
                            return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                        }

                        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);

                        using Stream stream = file.OpenReadStream();
                        blobClient.Upload(stream);
                        await blobClient.SetAccessTierAsync(AccessTier.Hot);

                        var fileUrl = blobClient.Uri.AbsoluteUri;
                        writer.Write(fileUrl);
                    }
                }
                catch (Exception ex)
                {
                    writer.Write(ex.Message);
                    return new JsonResult(new { StatusCode = 400, Message = ex.Message });
                }
            }

            return new JsonResult(new { StatusCode = 200 });
        }



    }
}

    

