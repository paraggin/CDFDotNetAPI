using Azure.Storage.Blobs;
using CDF_Core.Entities.Blob_Storage;
using CDF_Core.Entities.PNP_Accounts;
using CDF_Services.IServices.IBlobStorageService;
using CDF_Services.IServices.IPnpAccountServices;
using CDF_Services.Services.PnpAccountService;
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
        private const string _connectionString = "DefaultEndpointsProtocol=https;AccountName=blobpoc02;AccountKey=a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==;EndpointSuffix=core.windows.net";
        public BlobStorageController(IBlobStorageService BlobStorageService, IConfiguration configuration, Constants IConstants)
        {
            _BlobStorageService = BlobStorageService;
            _configuration = configuration;
            _IConstants = IConstants;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DownloadFile(string container_name, string prefix)
        {
            BlobClient blobClient = new BlobClient(_connectionString, container_name, prefix);
            using (var stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
                return File(stream.ToArray(), contentType, blobClient.Name);
            }
        }

        [HttpGet]
        [Route("ListBlobs")]
        public Task<IActionResult> ListBlobs(string container_name)
        {
            return _BlobStorageService.ListBlobs(container_name);
        }
    

    }
}
