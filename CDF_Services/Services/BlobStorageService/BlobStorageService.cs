using AutoMapper;
using Azure;
using Azure.Identity;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CDF_Core.Entities.Blob_Storage;
using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IBlobStorageService;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;


namespace CDF_Services.Services.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IGenericRepository<Blob_Storage> _IGenericRepository;
        private readonly IUnitOfWork<Blob_Storage> _IUnitOfWork;
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration, IGenericRepository<Blob_Storage> iGenericRepository, IUnitOfWork<Blob_Storage> iUnitOfWork, ApplicationDBContext dbContext, IMapper mapper)
        {
            _IGenericRepository = iGenericRepository;
            _IUnitOfWork = iUnitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<IActionResult> getBlobSasUrl_Dynamic(string accountName, string containerName, string blobName)
        {
            string sasUrl = "";
            try
            {

                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());

                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    BlobName = blobClient.Name,
                    ExpiresOn = DateTime.UtcNow.AddMinutes(15),
                    Protocol = SasProtocol.Https,
                    Resource = "b"
                };

                blobSasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Delete);

                UserDelegationKey userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(24));

                string sasToken = blobSasBuilder.ToSasQueryParameters(userDelegationKey, accountName).ToString();
                sasUrl = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";

                //  return new JsonResult(new { blobSASUrl = sasUrl });
                return new JsonResult(new { StatusCode = 200, Url = sasUrl });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { StatusCode = 400, Message = ex.Message, blobSASUrl = sasUrl });

            }

        }



        private string ProcessExcelStream(Stream stream)
        {
            try
            {
                // Register encoding provider
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                DataSet excelData;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    // Convert to DataSet with headers from the first row
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    excelData = reader.AsDataSet(conf);
                }

                // Convert DataSet to JSON
                var jsonResult = JsonConvert.SerializeObject(excelData, Formatting.Indented);
                return jsonResult;

            }
            catch (Exception ex)
            {
                return "Failed";
            }

        }
        async Task<string> getBLOBSasUrlForJsonFormat(string blobName)
        {
            string sasUrl = "";
            int statusCode = 200; // Default to 200 OK
            try
            {
                string accountName = _configuration["AzureBlobStorage:AccountName"];
                string containerName = _configuration["AzureBlobStorage:ContainerName"];

                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                // Check if the blob exists
                if (!await blobClient.ExistsAsync())
                {
                    statusCode = 400; // Blob not found
                    return statusCode.ToString();
                }

                BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    BlobName = blobClient.Name,
                    ExpiresOn = DateTime.UtcNow.AddMinutes(15),
                    Protocol = SasProtocol.Https,
                    Resource = "b"
                };

                blobSasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Delete);

                UserDelegationKey userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

                string sasToken = blobSasBuilder.ToSasQueryParameters(userDelegationKey, accountName).ToString();
                sasUrl = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";

                return sasUrl;
            }
            catch (Exception ex)
            {
                statusCode = 400;
                return statusCode.ToString();
            }
        }

        private string ProcessCSVStream(Stream stream)
        {
            try
            {
                // Register encoding provider
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                DataSet excelData;
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
                    // Convert to DataSet with headers from the first row
                    var conf = new ExcelDataSetConfiguration
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration
                        {
                            UseHeaderRow = true
                        }
                    };

                    excelData = reader.AsDataSet(conf);
                }

                var jsonResult = JsonConvert.SerializeObject(excelData, Formatting.Indented);
                return jsonResult;

            }
            catch (Exception ex)
            {
                return "Failed";
            }

        }


        public async Task<string> ConvertToJsonFromUrl(string fileName)
        {
            try
            {

                string url = await getBLOBSasUrlForJsonFormat(fileName);
                if (url == "400")
                {
                    return "File Not Found";
                }

                if (url != "")
                {

                    var extension = Path.GetExtension(url.Split("?")[0]).ToLower();


                    if (extension == ".xlsx" || extension == ".xls")
                    {
                        using (HttpClient client = new HttpClient())
                        {

                            var response = await client.GetAsync(url);
                            if (!response.IsSuccessStatusCode)
                            {
                                return "Failed";

                            }

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                return ProcessExcelStream(stream);

                            }
                        }
                    }
                    else if (extension == ".csv")
                    {
                        using (HttpClient client = new HttpClient())
                        {

                            var response = await client.GetAsync(url);
                            if (!response.IsSuccessStatusCode)
                            {
                                return "Failed";

                            }

                            using (var stream = await response.Content.ReadAsStreamAsync())
                            {
                                return ProcessCSVStream(stream);

                            }
                        }
                    }
                    else
                    {
                        return await textToJson(url);

                    }


                }
                else
                {
                    return "Failed";
                }


            }
            catch (HttpRequestException ex)
            {
                return "Failed";
            }
        }

        private async Task<string> textToJson(string url)
        {

            try
            {

                var headers = string.Empty;
                using (var httpClient = new HttpClient())
                {
                    // Fetch the content from the URL
                    var response = await httpClient.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        return "Failed";
                    }

                    var content = await response.Content.ReadAsStringAsync();


                    using (var reader = new StringReader(content))
                    {
                        headers = reader.ReadToEnd();

                    }
                }
                var jsonResult = headers;
                return jsonResult;
            }
            catch (Exception ex)
            {
                return "Failed";
            }
        }


        public async Task<IActionResult> uploadBlobMultiple(IFormFile file, int numberOfUploads = 10000, string? containerName = "blob-upload-cdf")
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



        public void ReceiveWebhook(dynamic payload)
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

            }
        }
        public async Task<IActionResult> downloadBlobTest(string prefix)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                string containerEndpoint = _configuration["AzureBlobStorage:ContainerEndpoint"];

                BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());

                BlobClient blobClient = containerClient.GetBlobClient(prefix);

                try
                {
                    using (var stream = new MemoryStream())
                    {
                        await blobClient.DownloadToAsync(stream);
                        stream.Position = 0;

                        var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;

                        return new FileContentResult(stream.ToArray(), contentType)
                        {
                            FileDownloadName = blobClient.Name
                        };
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine("Download Error 1  : " + ex.Message);
                }
                var stream1 = new MemoryStream();

                return new FileContentResult(stream1.ToArray(), "")
                {
                    FileDownloadName = blobClient.Name
                };

            }

        }
       public async Task<IActionResult> uploadDynamicBlobTest(IFormFile file)
        {

            var blobUrl = "";
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    if (file.Length > 0)
                    {
                        string ConnectionString = _configuration["AzureBlobStorage:ConnectionString"];

                        var fileName = Path.GetFileName(file.FileName);
                        var fileExtension = Path.GetExtension(fileName).ToLower(); ;

                        string containerEndpoint = _configuration["AzureBlobStorage:ContainerEndpoint"];

                        BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                                        new DefaultAzureCredential());

                        if (!await containerClient.ExistsAsync())
                        {
                            return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                        }

                        BlobClient blobClient = containerClient.GetBlobClient(fileName);
                        string contentType = GetContentType(fileExtension);
                        using Stream stream = file.OpenReadStream();
                        blobClient.Upload(stream, overwrite:true) ;
                       // await blobClient.SetAccessTierAsync(AccessTier.Hot);
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
                        blobUrl = fileUrl;
                    }
                    else
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "File Required" });
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
        public async Task<IActionResult> DeleteBlob(string fileName)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "File name cannot be null or empty." });
                    }

                    string ContainerName = _configuration["AzureBlobStorage:ContainerName"];

                    string storageAccountName = _configuration["AzureBlobStorage:AccountName"];
                    string containerEndpoint = $"https://{storageAccountName}.blob.core.windows.net/{ContainerName}/";


                    BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                                   new DefaultAzureCredential());

                    if (!await containerClient.ExistsAsync())
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                    }

                    // Get a reference to the blob
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);

                    // Delete the blob if it exists
                    var deleteResponse = await blobClient.DeleteIfExistsAsync();

                    if (deleteResponse.Value)
                    {
                        writer.WriteLine("---------------" + DateTime.Now + "---------------");
                        writer.WriteLine($"Blob {fileName} deleted successfully.");
                    }
                    else
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "Blob not found or already deleted." });
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");
                    writer.WriteLine(ex.Message);
                    return new JsonResult(new { StatusCode = 400, Message = ex.Message });
                }
            }

            return new JsonResult(new { StatusCode = 200, Message = "Blob deleted successfully." });
        }



        private string GetContentType(string fileExtension)
        {
            switch (fileExtension)
            {
                // Text formats
                case ".txt":
                    return "text/plain";
                case ".csv":
                    return "text/csv";
                case ".html":
                case ".htm":
                    return "text/html";
                case ".css":
                    return "text/css";
                case ".json":
                    return "application/json";
                case ".xml":
                    return "application/xml";

                // Image formats
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".tiff":
                case ".tif":
                    return "image/tiff";
                case ".svg":
                    return "image/svg+xml";
                case ".ico":
                    return "image/x-icon";

                // Document formats
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".rtf":
                    return "application/rtf";

                // Compressed formats
                case ".zip":
                    return "application/zip";
                case ".rar":
                    return "application/vnd.rar";
                case ".7z":
                    return "application/x-7z-compressed";
                case ".tar":
                    return "application/x-tar";
                case ".gz":
                    return "application/gzip";

                // Audio formats
                case ".mp3":
                    return "audio/mpeg";
                case ".wav":
                    return "audio/wav";
                case ".ogg":
                    return "audio/ogg";
                case ".m4a":
                    return "audio/x-m4a";
                case ".flac":
                    return "audio/flac";

                // Video formats
                case ".mp4":
                    return "video/mp4";
                case ".avi":
                    return "video/x-msvideo";
                case ".mov":
                    return "video/quicktime";
                case ".wmv":
                    return "video/x-ms-wmv";
                case ".mkv":
                    return "video/x-matroska";
                case ".webm":
                    return "video/webm";

                // Font formats
                case ".ttf":
                    return "font/ttf";
                case ".otf":
                    return "font/otf";
                case ".woff":
                    return "font/woff";
                case ".woff2":
                    return "font/woff2";

                // Binary formats
                case ".exe":
                    return "application/octet-stream";
                case ".dll":
                    return "application/octet-stream";
                case ".bin":
                    return "application/octet-stream";

                // Others
                case ".eot":
                    return "application/vnd.ms-fontobject";
                case ".swf":
                    return "application/x-shockwave-flash";

                // Default
                default:
                    return "application/octet-stream"; // Default for unknown types
            }
        }

        public async Task<IActionResult> uploadBlob(IFormFile file, string containerName)
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


        public async Task<IActionResult> getBLobSAS(string BlobName)
        {

            string AccountName = _configuration["AzureBlobStorage:AccountName"];
            string AccountKey = _configuration["AzureBlobStorage:AccountKey"];
            string ContainerName = _configuration["AzureBlobStorage:ContainerName"];
            string ConnectionString = _configuration["AzureBlobStorage:ConnectionString"];
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                BlobContainerClient blobContainerClient = new BlobContainerClient(ConnectionString,
                ContainerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(BlobName);

                Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
                {
                    BlobContainerName = ContainerName,
                    BlobName = BlobName,
                    ExpiresOn = DateTime.UtcNow.AddMinutes(15),
                    Protocol = SasProtocol.Https,
                    Resource = "c",
                };
                //blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
                blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.All);
                //blobSasBuilder.SetPermissions(BlobContainerSasPermissions.All);
                // blobSasBuilder.SetPermissions(BlobContainerSasPermissions.Write);

                var sasToken = blobSasBuilder.ToSasQueryParameters(new
                StorageSharedKeyCredential(AccountName, AccountKey)).ToString();
                var sasURL = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";
                writer.WriteLine("---------------" + DateTime.Now + "---------------");

                writer.WriteLine(sasURL);
                return new JsonResult(new { blobSASUrl = sasURL });

            }

        }


        public async Task<IActionResult> getBLobSASIdentity(string blobName)
        {
            string accountName = _configuration["AzureBlobStorage:AccountName"];

            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), new DefaultAzureCredential());

                    string containerName = _configuration["AzureBlobStorage:ContainerName"];
                    BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                    BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = blobContainerClient.Name,
                        BlobName = blobClient.Name,
                        ExpiresOn = DateTime.UtcNow.AddMinutes(15),
                        Protocol = SasProtocol.Https,
                        Resource = "b"  // 'b' indicates Blob-level resource in the container
                    };

                    blobSasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Delete);

                    UserDelegationKey userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

                    string sasToken = blobSasBuilder.ToSasQueryParameters(userDelegationKey, accountName).ToString();
                    string sasUrl = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";

                    writer.WriteLine("---------------" + DateTime.Now + "---------------");
                    writer.WriteLine(sasUrl);

                    return new JsonResult(new { blobSasUrl = sasUrl });
                }
                catch (Exception e)
                {
                    writer.WriteLine("Error :" + e.ToString());
                    return new JsonResult(new { StatusCode = 400, Message = "Error :" + e.ToString() });
                }
            }
        }

       
        public async Task<IActionResult> ListBlobs_Identity(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {

            try
            {
                string accountName = _configuration["AzureBlobStorage:AccountName"];

                var credential = new DefaultAzureCredential();
                var serviceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);

                containerName = containerName == "" ? _configuration["AzureBlobStorage:ContainerName"] : containerName;
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
                int totalCount = 0;

                string query = "";
                string startdate = "";
                string enddate = "";
                if (!string.IsNullOrEmpty(period))
                {
                    if (DateTime.TryParseExact(period, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        DateTime fromDate = new DateTime(parsedDate.Year, parsedDate.Month, 1);
                        DateTime toDate = fromDate.AddMonths(1).AddDays(-1);
                        startdate = fromDate.ToString("yyyy-MM-dd");
                        enddate = toDate.ToString("yyyy-MM-dd");
                    }
                }

                if (!string.IsNullOrEmpty(startdate))
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";

                    query += @$"""period"" >= '{startdate}'";
                }

                if (!string.IsNullOrEmpty(enddate))
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";

                    query += @$"""period"" <= '{enddate}'";
                }

                if (!string.IsNullOrEmpty(reportingUnit))
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";

                    query += @$"""reportingunit"" = '{reportingUnit}'";
                }

                using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");
                    writer.WriteLine("Q : " + query);

                }
               

                string continuationToken = null;
                int skip = (int)((pageNumber - 1) * pageSize);
                //  skip = 0;
                List<Object> blobTags = new List<Object>();
                if (!string.IsNullOrEmpty(query))
                {

                    if (!string.IsNullOrEmpty(filename))
                    {
                        totalCount = 0;
                        await foreach (TaggedBlobItem blobItem in containerClient.FindBlobsByTagsAsync(query))
                        {
                            if (blobItem.BlobName.Contains(filename, StringComparison.OrdinalIgnoreCase))
                            {

                                totalCount++;
                            }

                        }

                        await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query).AsPages(continuationToken, pageSize))
                        {

                            foreach (TaggedBlobItem blobItem in page.Values)
                            {
                                if (blobItem.BlobName.Contains(filename, StringComparison.OrdinalIgnoreCase))
                                {
                                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.BlobName);
                                    GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                                    blobTags.Add(new { name = blobItem.BlobName, tags = blobProperties.Tags });
                                }

                                if (blobTags.Count >= pageSize)
                                {
                                    break;
                                }
                            }

                            if ((int)skip >= (int)page.Values.Count)
                            {
                                skip -= page.Values.Count;
                                continuationToken = page.ContinuationToken;
                                continue;
                            }
                            if (blobTags.Count >= pageSize)
                            {
                                break;
                            }

                            skip = Math.Max(0, pageSize - blobTags.Count);

                            continuationToken = page.ContinuationToken;
                        }
                    }
                    else
                    {
                        await foreach (TaggedBlobItem blob in containerClient.FindBlobsByTagsAsync(query))
                        {
                            totalCount++;
                        }
                        await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query).AsPages(continuationToken, pageSize))
                        {
                            if (skip >= page.Values.Count)
                            {
                                skip -= page.Values.Count;
                                continuationToken = page.ContinuationToken;
                                continue;
                            }

                            var items = page.Values.Skip(skip);

                            foreach (TaggedBlobItem blobItem in items)
                            {
                                BlobClient blobClient = containerClient.GetBlobClient(blobItem.BlobName);
                                GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                                blobTags.Add(new { name = blobItem.BlobName, tags = blobProperties.Tags });
                            }

                            if (blobTags.Count >= pageSize)
                            {
                                break;
                            }

                            skip = Math.Max(0, (int)(pageSize - blobTags.Count));
                            continuationToken = page.ContinuationToken;
                        }
                    }

                    return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });
                }

                await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
                {
                    totalCount++;
                }
                await foreach (Page<BlobItem> page in containerClient.GetBlobsAsync(traits: BlobTraits.Metadata, prefix: filename).AsPages(continuationToken, pageSize))
                {
                    if (skip >= page.Values.Count)
                    {
                        skip -= page.Values.Count;
                        continuationToken = page.ContinuationToken;
                        continue;
                    }

                    var items = page.Values.Skip(skip);

                    foreach (BlobItem blobItem in items)
                    {
                        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                        GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                        blobTags.Add(new { name = blobItem.Name, tags = blobProperties.Tags });
                    }

                    if (blobTags.Count >= pageSize)
                    {
                        break;
                    }

                    skip = Math.Max(0, (int)pageSize - blobTags.Count);
                    continuationToken = page.ContinuationToken;
                }

                return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Status = 400, Message = ex.ToString() });

            }
        }
        public async Task<IActionResult> ListBlobsUsingMetadata_Identity(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {
            try
            {
                string accountName = _configuration["AzureBlobStorage:AccountName"];

                var credential = new DefaultAzureCredential();
                var serviceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);

                containerName = containerName == "" ? _configuration["AzureBlobStorage:ContainerName"] : containerName;
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
                int totalCount = 0;

                string continuationToken = null;
                int skip = (int)((pageNumber - 1) * pageSize);
                List<Object> blobTags = new List<Object>();

                // Construct the metadata filter query
               /* string metadataFilter = "";
                if (!string.IsNullOrEmpty(filename))
                {
                    metadataFilter = $"metadata/fileName eq '{filename}'";
                }*/

                // Fetch blobs using metadata filtering
                await foreach (Page<BlobItem> page in containerClient.GetBlobsAsync(traits: BlobTraits.Metadata, prefix: filename).AsPages(continuationToken, pageSize))
                {
                    if (skip >= page.Values.Count)
                    {
                        skip -= page.Values.Count;
                        continuationToken = page.ContinuationToken;
                        continue;
                    }

                    var items = page.Values.Skip(skip);

                    foreach (BlobItem blobItem in items)
                    {
                        // Get blob metadata
                        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                        Response<BlobProperties> blobProperties = await blobClient.GetPropertiesAsync();
                        blobTags.Add(new { name = blobItem.Name, tags = blobProperties.Value.Metadata });
                        // Check if metadata matches the filter
                        if (blobProperties.Value.Metadata.TryGetValue("period", out var periodValue) &&
                            blobProperties.Value.Metadata.TryGetValue("reportingunit", out var reportingUnitValue) &&
                            (string.IsNullOrEmpty(period) || periodValue == period) &&
                            (string.IsNullOrEmpty(reportingUnit) || reportingUnitValue == reportingUnit))
                        {
                           // blobTags.Add(new { name = blobItem.Name, tags = blobProperties.Value.Metadata });
                        }
                    }

                    if (blobTags.Count >= pageSize)
                    {
                        break;
                    }

                    skip = Math.Max(0, (int)(pageSize - blobTags.Count));
                    continuationToken = page.ContinuationToken;
                }

                // Count total blobs
                totalCount = blobTags.Count;

                return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Status = 400, Message = ex.ToString() });
            }
        }


        public async Task<IActionResult> FilterBlobs(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {

            try
            {
                string connectionString = _configuration["AzureBlobStorage:ConnectionString"];

                // var serviceClient = new BlobServiceClient(connectionString);


                var credential = new DefaultAzureCredential();
                var serviceClient = new BlobServiceClient(new Uri("https://blobpoc02.blob.core.windows.net"), credential);



                containerName = containerName == "" ? _configuration["AzureBlobStorage:ContainerName"] : containerName;
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
                int totalCount = 0;


                // period = string.IsNullOrEmpty(period) ? DateTime.Now.ToString("MM-yyyy") : period;

                string query = "";
                string startdate = "";
                string enddate = "";
                if (!string.IsNullOrEmpty(period))
                {
                    if (DateTime.TryParseExact(period, "MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                    {
                        DateTime fromDate = new DateTime(parsedDate.Year, parsedDate.Month, 1);
                        DateTime toDate = fromDate.AddMonths(1).AddDays(-1);
                        startdate = fromDate.ToString("yyyy-MM-dd");
                        enddate = toDate.ToString("yyyy-MM-dd");
                    }
                }

                if (!string.IsNullOrEmpty(startdate))
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";

                    query += @$"""period"" >= '{startdate}'";
                }

                if (!string.IsNullOrEmpty(enddate))
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";

                    query += @$"""period"" <= '{enddate}'";
                }

                if (!string.IsNullOrEmpty(reportingUnit))
                {
                    if (!string.IsNullOrEmpty(query))
                        query += " AND ";

                    query += @$"""reportingunit"" = '{reportingUnit}'";
                }

                using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");
                    writer.WriteLine("Q : " + query);
                }

                string continuationToken = null;
                int skip = (int)((pageNumber - 1) * pageSize);
                //  skip = 0;
                List<Object> blobTags = new List<Object>();
                if (!string.IsNullOrEmpty(query))
                {

                    if (!string.IsNullOrEmpty(filename))
                    {
                        totalCount = 0;
                        await foreach (TaggedBlobItem blobItem in containerClient.FindBlobsByTagsAsync(query))
                        {
                            if (blobItem.BlobName.Contains(filename, StringComparison.OrdinalIgnoreCase))
                            {

                                totalCount++;
                            }

                        }

                        await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query).AsPages(continuationToken, pageSize))
                        {

                            foreach (TaggedBlobItem blobItem in page.Values)
                            {
                                if (blobItem.BlobName.Contains(filename, StringComparison.OrdinalIgnoreCase))
                                {
                                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.BlobName);
                                    GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                                    blobTags.Add(new { name = blobItem.BlobName, tags = blobProperties.Tags });
                                }

                                if (blobTags.Count >= pageSize)
                                {
                                    break;
                                }
                            }

                            if ((int)skip >= (int)page.Values.Count)
                            {
                                skip -= page.Values.Count;
                                continuationToken = page.ContinuationToken;
                                continue;
                            }
                            if (blobTags.Count >= pageSize)
                            {
                                break;
                            }

                            skip = Math.Max(0, pageSize - blobTags.Count);

                            continuationToken = page.ContinuationToken;
                        }
                    }
                    else
                    {
                        await foreach (TaggedBlobItem blob in containerClient.FindBlobsByTagsAsync(query))
                        {
                            totalCount++;
                        }
                        await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query).AsPages(continuationToken, pageSize))
                        {
                            if (skip >= page.Values.Count)
                            {
                                skip -= page.Values.Count;
                                continuationToken = page.ContinuationToken;
                                continue;
                            }

                            var items = page.Values.Skip(skip);

                            foreach (TaggedBlobItem blobItem in items)
                            {
                                BlobClient blobClient = containerClient.GetBlobClient(blobItem.BlobName);
                                GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                                blobTags.Add(new { name = blobItem.BlobName, tags = blobProperties.Tags });
                            }

                            if (blobTags.Count >= pageSize)
                            {
                                break;
                            }

                            skip = Math.Max(0, (int)(pageSize - blobTags.Count));
                            continuationToken = page.ContinuationToken;
                        }
                    }

                    return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });
                }

                await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
                {
                    totalCount++;
                }
                await foreach (Page<BlobItem> page in containerClient.GetBlobsAsync(traits: BlobTraits.Metadata, prefix: filename).AsPages(continuationToken, pageSize))
                {
                    if (skip >= page.Values.Count)
                    {
                        skip -= page.Values.Count;
                        continuationToken = page.ContinuationToken;
                        continue;
                    }

                    var items = page.Values.Skip(skip);

                    foreach (BlobItem blobItem in items)
                    {
                        BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                        GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                        blobTags.Add(new { name = blobItem.Name, tags = blobProperties.Tags });
                    }

                    if (blobTags.Count >= pageSize)
                    {
                        break;
                    }

                    skip = Math.Max(0, (int)pageSize - blobTags.Count);
                    continuationToken = page.ContinuationToken;
                }

                return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { Status = 400, Message = ex.ToString() });

            }
        }



        public async Task<IActionResult> FilterBlobsUsingRestAPI(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {
            containerName = containerName == "" ? _configuration["AzureBlobStorage:ContainerName"] : containerName;

            string baseUrl = $"https://blobpoc02.blob.core.windows.net";
            string url = $"{baseUrl}/{containerName}?restype=container&comp=list&prefix={filename}";

            if (!string.IsNullOrEmpty(period))
            {
                // Assuming period is used as a prefix for filtering blobs
                url += $"&prefix={period}";
            }

            if (!string.IsNullOrEmpty(reportingUnit))
            {
                // Assuming reporting unit is used as metadata for filtering blobs
                url += $"&include=metadata&metadata=reportingunit:{reportingUnit}";
            }

            using (HttpClient client = new HttpClient())
            {
                // Add storage account key for authentication
                string storageAccountKey = _configuration["AzureBlobStorage:AccountKey"];
                client.DefaultRequestHeaders.Add("x-ms-version", "2020-06-12");
                client.DefaultRequestHeaders.Add("Authorization", $"SharedKey blobpoc02:{storageAccountKey}");

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    dynamic result = JsonConvert.DeserializeObject(json);

                    // Parse the response to extract blob information
                    // This depends on the structure of the response from the REST API

                    return new JsonResult(result);
                }
                else
                {
                    // Handle the case when the request fails
                    return new JsonResult(response);
                }
            }
        }
        public async Task<IActionResult> FilterBlobsUsingRestAPI1(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {
            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            containerName = containerName == "" ? _configuration["AzureBlobStorage:ContainerName"] : containerName;

            string baseUrl = $"https://{containerName}.blob.core.windows.net";
            string url = $"https://blobpoc02.blob.core.windows.net/{containerName}?restype=container&comp=list&prefix={filename}";

            if (!string.IsNullOrEmpty(period))
            {
                url += $"&prefix={period}";
            }

            if (!string.IsNullOrEmpty(reportingUnit))
            {
                url += $"&include=metadata&metadata=reportingunit:{reportingUnit}";
            }

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("x-ms-version", "2020-06-12");

                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    // dynamic result = JsonConvert.DeserializeObject(json);

                    return new JsonResult(json);
                }
                else
                {
                    // Handle the case when the request fails
                    return new JsonResult(response);

                }
            }
        }


        private static async void ListContainersAsyncREST(string storageAccountName, string storageAccountKey, CancellationToken cancellationToken)
        {
            // Construct the URI. This will look like this:
            //   https://blolpoc02.blob.core.windows.net/resource
            // string uri = $"http://{storageAccountName}.blob.core.windows.net?comp=list";
            string uri = $"https://blobpoc02.blob.core.windows.net/container-test?restype=container&comp=list";


            // Set this to whatever payload you desire. Ours is null because 
            //   we're not passing anything in.
            byte[] requestPayload = null;

            // Instantiate the request message with a null payload.
            using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri)
            {
                Content = requestPayload == null ? null : new ByteArrayContent(requestPayload)
            };

            // Add the request headers for x-ms-date and x-ms-version.
            DateTime now = DateTime.UtcNow;
            httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
            httpRequestMessage.Headers.Add("x-ms-version", "2017-04-17");
            // If you need any additional headers, add them here before creating
            //   the authorization header. 

            // Add the authorization header.
            httpRequestMessage.Headers.Authorization = AzureStorageAuthenticationHelper.GetAuthorizationHeader(
               storageAccountName, storageAccountKey, now, httpRequestMessage);

            // Send the request.
            using var httpClient = new HttpClient();
            using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, cancellationToken);

            // If successful (status code = 200), 
            //   parse the XML response for the container names.
            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
            {
                string xmlString = await httpResponseMessage.Content.ReadAsStringAsync();
                XElement x = XElement.Parse(xmlString);
                foreach (XElement container in x.Element("Containers").Elements("Container"))
                {
                    Console.WriteLine($"Container name = {container.Element("Name").Value}");
                }
            }

        }
        private static string GetRelativePathFromUri(Uri uri)
        {
            return uri.AbsolutePath;
        }

        private static string GetCanonicalizedHeaders(HttpRequestMessage request)
        {
            StringBuilder headers = new StringBuilder();
            foreach (var header in request.Headers.OrderBy(h => h.Key.ToLowerInvariant()))
            {
                headers.Append($"{header.Key.ToLowerInvariant()}:{string.Join(",", header.Value)}\n");
            }
            return headers.ToString();
        }
        private static string GetSharedKeyAuthorizationHeader(string storageAccountName, string storageAccountKey, DateTime now, HttpRequestMessage request)
        {
            string canonicalizedResource = $"/{storageAccountName}/{GetRelativePathFromUri(request.RequestUri)}";

            // Construct canonicalized headers
            string canonicalizedHeaders = GetCanonicalizedHeaders(request);

            // Get values for each header or use empty string if the header is not present
            string method = request.Method.Method;
            string date = request.Headers.Contains("Date") ? request.Headers.GetValues("Date").FirstOrDefault() : "";
            string contentType = request.Content?.Headers?.ContentType?.ToString() ?? "";
            string contentLength = request.Content?.Headers?.ContentLength?.ToString() ?? "";

            string stringToSign = $"{method}\n\n\n{contentLength}\n\n{contentType}\n{date}\n{canonicalizedHeaders}{canonicalizedResource}";

            using (var hmac = new HMACSHA256(Convert.FromBase64String(storageAccountKey)))
            {
                byte[] data = Encoding.UTF8.GetBytes(stringToSign);
                string signature = Convert.ToBase64String(hmac.ComputeHash(data));
                return $"{storageAccountName}:{signature}";
            }
        }

        public async Task<IActionResult> DownloadFile(string prefix)
        {
            BlobClient blobClient = new BlobClient(_configuration["AzureBlobStorage:ConnectionString"], _configuration["AzureBlobStorage:ContainerName"], prefix);
            using (var stream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;

                return new FileContentResult(stream.ToArray(), contentType)
                {
                    FileDownloadName = blobClient.Name
                };
            }
        }

    }


}
