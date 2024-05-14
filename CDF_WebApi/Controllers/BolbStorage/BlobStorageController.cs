using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        [HttpPost]
        [Route("webhook")]
        public IActionResult ReceiveWebhook([FromBody] dynamic payload)
        {
            try
            {
                // Parse the incoming JSON payload
                string jsonString = JsonConvert.SerializeObject(payload);

                // Process the webhook data
                // Example: Log it
                using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                {
                    Console.WriteLine("Received webhook payload: " + jsonString);
                    writer.WriteLine("Received webhook payload: " + jsonString);
                }
                // Add your custom processing logic here

                // Respond with a success status code
                return Ok();
            }
            catch (Exception ex)
            {
                using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                {
                    writer.WriteLine("Error processing webhook: " + ex.Message);
                }
                // Log any errors
                Console.WriteLine("Error processing webhook: " + ex.Message);

                // Respond with an error status code
                return StatusCode(500);
            }
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

           // string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");
            string logFilePath = Path.Combine(AppContext.BaseDirectory, "log.txt");

            //  using (StreamWriter writer = new StreamWriter(logFilePath, true))
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
                        var propertiesJson = JsonConvert.SerializeObject(blobClient.GetProperties(), Formatting.Indented);
                        var tagsJson = JsonConvert.SerializeObject(blobClient.GetTags(), Formatting.Indented);
                        var typeJson = JsonConvert.SerializeObject(blobClient.GetType(), Formatting.Indented);

                        // Write JSON strings to the log file
                        writer.WriteLine("---------------" + DateTime.Now + "---------------");
                        writer.WriteLine("Properties : " + propertiesJson + "---------------");
                        writer.WriteLine("Tags : " + tagsJson + "---------------");
                        writer.WriteLine("Type : " + typeJson + "---------------");



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

    

