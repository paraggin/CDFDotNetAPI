using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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
            //return await _BlobStorageService.FilterBlobsUsingRestAPI((int)pageSize, (int)pageNumber, period, reportingUnit, filename, containerName);

        }

        [HttpGet]
        [Route("ListBlobsv3")]
        public async Task<IActionResult> ListBlobsv3(int? pageSize = 10)
        {
            return await _BlobStorageService.ListBlobsAsyncREST();

        }

        //https://blobpoc02.blob.core.windows.net/container-poc/
        [HttpPost]
        [Route("uploadBlobTest")]
        public async Task<IActionResult> uploadBlobTest()
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                string blobContents = "Testing identity";
                string containerEndpoint = "https://blobpoc02.blob.core.windows.net/container-poc/";

                // Get a credential and create a client object for the blob container.
                BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                                new DefaultAzureCredential());

                try
                {
                    // Create the container if it does not exist.
                    await containerClient.CreateIfNotExistsAsync();

                    // Upload text to a new block blob.
                    byte[] byteArray = Encoding.ASCII.GetBytes(blobContents);

                    using (MemoryStream stream = new MemoryStream(byteArray))
                    {
                        await containerClient.UploadBlobAsync("Test_F1", stream);
                    }

                    return new JsonResult(new { StatusCode = 200, Message = "Success" });

                }
                catch (Exception e)
                {
                    writer.WriteLine("Identity Error :"+e.ToString());
                    return new JsonResult(new { StatusCode = 400,Message ="Identity Error :" + e.ToString() });


                }


            }

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
                         var fileExtension = Path.GetExtension(fileName).ToLower(); ;

                        BlobContainerClient blobContainerClient = new BlobContainerClient(ConnectionString, containerName);

                        if (!await blobContainerClient.ExistsAsync())
                        {
                            return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                        }

                        BlobClient blobClient = blobContainerClient.GetBlobClient(fileName);
                        string contentType = GetContentType(fileExtension);
                        using Stream stream = file.OpenReadStream();
                        blobClient.Upload(stream);
                        await blobClient.SetAccessTierAsync(AccessTier.Hot);
                        await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });


                        IDictionary<string, string> tags = new Dictionary<string, string>
                        {
                            { "file",fileName },
                            { "period" ,DateTime.Now.ToString("yyyy-MM-dd")}
                        };

                        await blobClient.SetTagsAsync(tags);
                        var fileUrl = blobClient.Uri.AbsoluteUri;
                        writer.WriteLine("---------------" + DateTime.Now + "---------------");

                        writer.Write(fileUrl);
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");

                    writer.Write(ex.Message);
                    return new JsonResult(new { StatusCode = 400, Message = ex.Message });
                }
            }

            return new JsonResult(new { StatusCode = 200 });
        }

        [HttpPost]
        [Route("uploadBlobMultiple")]
        public async Task<IActionResult> uploadBlobMultiple(IFormFile file, int numberOfUploads = 10000, string? containerName = "container-poc")
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    for (int i = 0; i < numberOfUploads; i++)
                    {
                        if (file.Length > 0)
                        {
                            string ConnectionString = _configuration["AzureBlobStorage:ConnectionString"];

                            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var fileExtension = Path.GetExtension(file.FileName).ToLower();

                            BlobContainerClient blobContainerClient = new BlobContainerClient(ConnectionString, containerName);

                            if (!await blobContainerClient.ExistsAsync())
                            {
                                return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                            }

                            // Append an incremented digit to the filename
                            //  string uniqueFileName = $"{fileName}_{i}{fileExtension}";
                            string uniqueFileName = $"{i}{fileExtension}";
                            BlobClient blobClient = blobContainerClient.GetBlobClient(uniqueFileName);

                            string contentType = GetContentType(fileExtension);
                            using Stream stream = file.OpenReadStream();
                            blobClient.Upload(stream);
                            await blobClient.SetAccessTierAsync(AccessTier.Hot);
                            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders { ContentType = contentType });

                            IDictionary<string, string> tags = new Dictionary<string, string>
                    {
                        { "file", file.FileName },
                        { "period" ,DateTime.Now.ToString("yyyy-MM-dd")}
                    };

                            await blobClient.SetTagsAsync(tags);
                            var fileUrl = blobClient.Uri.AbsoluteUri;
                            writer.WriteLine("---------------" + DateTime.Now + "---------------");

                            writer.Write(fileUrl);
                        }
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");

                    writer.Write(ex.Message);
                    return new JsonResult(new { StatusCode = 400, Message = ex.Message });
                }
            }

            return new JsonResult(new { StatusCode = 200 });
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension)
            {
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".pdf":
                    return "application/pdf";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                default:
                    return "application/octet-stream";
            }
        }


    }
}

    

