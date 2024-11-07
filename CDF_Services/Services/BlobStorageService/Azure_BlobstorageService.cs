using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CDF_Services.IServices.IBlobStorageService;
using ExcelDataReader;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Files.DataLake;
using CsvHelper;
using System.Globalization;

namespace CDF_Services.Services.BlobStorageService
{
    public class Azure_BlobstorageService : IAzure_BlobstorageService
    {
        private readonly IConfiguration _configuration;

        public Azure_BlobstorageService(IConfiguration configuration)
        {

            _configuration = configuration;
        }
        private string ProcessCSVStream1(Stream stream)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                DataSet excelData;
                using (var reader = ExcelReaderFactory.CreateCsvReader(stream))
                {
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
                return $"Failed 3 : {ex.ToString()}";
            }

        }
        private async Task<string> ProcessCSVStream(Stream stream)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                using (var reader = new StreamReader(stream))
                {
                    var csvData = new List<Dictionary<string, string>>();
                    var header = await reader.ReadLineAsync(); // Read the header line
                    var headers = header.Split(',');

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line == null) continue;

                        var values = line.Split(',');
                        var rowData = new Dictionary<string, string>();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            rowData[headers[i]] = values.Length > i ? values[i] : null;
                        }
                        csvData.Add(rowData);

                        // Optionally: Process each row immediately to free memory
                        // ProcessRow(rowData);
                    }

                    // Convert the whole list to JSON only if necessary
                    var jsonResult = JsonConvert.SerializeObject(csvData, Formatting.Indented);
                    return jsonResult;
                }
            }
            catch (Exception ex)
            {
                return $"Failed 3 : {ex.ToString()}";
            }
        }

        private string ProcessExcelStream(Stream stream)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

                DataSet excelData;
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
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
                return $"Failed 4 : {ex.ToString()}";
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
                return $"Failed 5 : {ex.ToString()}";
            }
        }

        async Task<string> getBLOBSasUrlForJsonFormat(string blobName)
        {
            string sasUrl = "";
            int statusCode = 200; // Default to 200 OK
            try
            {
                string accountName = _configuration["Azure-Blobstorage:AccountName"];
                string containerName = _configuration["Azure-Blobstorage:ContainerName"];

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
                    ExpiresOn = DateTime.UtcNow.AddHours(24),
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

        private async Task<string> ProcessCSVStream_CsvHelper(Stream stream)
        {
            try
            {
                var csvData = new List<dynamic>();
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    while (await csv.ReadAsync())
                    {
                        var record = csv.GetRecord<dynamic>();
                        csvData.Add(record);
                    }
                }

                var jsonResult = JsonConvert.SerializeObject(csvData, Formatting.Indented);
                return jsonResult;
            }
            catch (Exception ex)
            {
                return $"Failed: {ex}";
            }
        }


        public async Task<string> ConvertToJsonFromUrl(string fileName)
        {
            try
            {

                //  string url = "https://demohierarchical.blob.core.windows.net/sftp-esker/customers-500000.csv?skoid=fa2099ba-f7dd-4ef2-9b01-8169b911667a&sktid=f9527b8e-12ef-47fc-85aa-443d41b6f525&skt=2024-10-15T10%3A38%3A15Z&ske=2024-10-15T11%3A38%3A15Z&sks=b&skv=2023-11-03&sv=2023-11-03&spr=https&st=2024-10-15T10%3A37%3A15Z&se=2024-10-15T10%3A53%3A15Z&sr=b&sp=rwd&sig=8UqV%2Bax9YqEuwZpdeBYxi4N2tnD8jyKrSOFz5OEc%2F0g%3D";
                string url = await getBLOBSasUrlForJsonFormat(fileName);
                //string url = "https://demohierarchical.blob.core.windows.net/sftp-esker/testpopulation.csv?skoid=fa2099ba-f7dd-4ef2-9b01-8169b911667a&sktid=f9527b8e-12ef-47fc-85aa-443d41b6f525&skt=2024-10-15T10%3A56%3A12Z&ske=2024-10-15T11%3A56%3A12Z&sks=b&skv=2023-11-03&sv=2023-11-03&spr=https&st=2024-10-15T10%3A55%3A12Z&se=2024-10-15T11%3A11%3A12Z&sr=b&sp=rwd&sig=VXxmmsUjy9ylF4%2FpOyMSNjzIqwBqCutmwPgCqtfU8kU%3D";
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
                                return await ProcessCSVStream_CsvHelper(stream);

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
        public async Task<IActionResult> downloadBlob(string name)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    string accountName = _configuration["Azure-Blobstorage:AccountName"];
                    string containerName = _configuration["Azure-Blobstorage:ContainerName"];

                    string containerEndpoint = $"https://{accountName}.blob.core.windows.net/{containerName}/";


                    BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());

                    BlobClient blobClient = containerClient.GetBlobClient(name);

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
                        writer.WriteLine("Download Error   : " + ex.Message);
                        return new JsonResult(new { Status = 400, Message = ex.ToString() });

                    }


                }
                catch (Exception e)
                {
                    return new JsonResult(new { Status = 400, Message = e.ToString() });
                }

            }
        }

        public async Task<IActionResult> FilterBlobs(int pageSize, int pageNumber)
        {

            try
            {
                string accountName = _configuration["Azure-Blobstorage:AccountName"];
                string containerName = _configuration["Azure-Blobstorage:ContainerName"];


                var credential = new DefaultAzureCredential();
                var serviceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);
                BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);


                int totalCount = 0;
                List<Object> blobTags = new List<Object>();
                await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
                {
                    totalCount++;
                    BlobClient blobClient = containerClient.GetBlobClient(blob.Name);
                    // GetBlobTagResult blobProperties = await blobClient.GetTagsAsync();
                    // blobTags.Add(new { name = blob.Name, tags = blobProperties.Tags });
                    blobTags.Add(new { name = blob.Name });

                }

                return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { Status = 400, Message = ex.ToString() });

            }
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

                    string ContainerName = _configuration["Azure-Blobstorage:ContainerName"];

                    string storageAccountName = _configuration["Azure-Blobstorage:AccountName"];
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
        public async Task<IActionResult> uploadBlob(IFormFile file)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                     (sender, cert, chain, sslPolicyErrors) => true;

                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var fileExtension = Path.GetExtension(fileName).ToLower();

                        string storageAccountName = _configuration["Azure-Blobstorage:AccountName"];
                        string fileSystemName = _configuration["Azure-Blobstorage:ContainerName"]; // File system (container) name for Data Lake
                        string fileSystemEndpoint = $"https://{storageAccountName}.dfs.core.windows.net/{fileSystemName}/";

                        DataLakeFileSystemClient fileSystemClient = new DataLakeFileSystemClient(new Uri(fileSystemEndpoint), new DefaultAzureCredential());

                        if (!await fileSystemClient.ExistsAsync())
                        {
                            return new JsonResult(new { StatusCode = 400, Message = "File system does not exist." });
                        }

                        DataLakeFileClient fileClient = fileSystemClient.GetFileClient(fileName);

                        using Stream stream = file.OpenReadStream();
                        await fileClient.UploadAsync(stream, overwrite: true);

                        string contentType = GetContentType(fileExtension);
                        await fileClient.SetHttpHeadersAsync(new Azure.Storage.Files.DataLake.Models.PathHttpHeaders { ContentType = contentType });

                        IDictionary<string, string> metadata = new Dictionary<string, string>
                    {
                        { "file", fileName },
                        { "period", DateTime.Now.ToString("yyyy-MM-dd") }
                    };
                        await fileClient.SetMetadataAsync(metadata);

                        var fileUrl = fileClient.Uri.AbsoluteUri;
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

            return new JsonResult(new { StatusCode = 200, Message = "File uploaded successfully." });
        }


        public async Task<IActionResult> uploadBlob_old(IFormFile file)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                     (sender, cert, chain, sslPolicyErrors) => true;

                    if (file.Length > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var fileExtension = Path.GetExtension(fileName).ToLower();

                        string storageAccountName = _configuration["Azure-Blobstorage:AccountName"];
                        string fileSystemName = _configuration["Azure-Blobstorage:ContainerName"]; // File system (container) name for Data Lake
                        string fileSystemEndpoint = $"https://{storageAccountName}.dfs.core.windows.net/{fileSystemName}/";

                        // Create DataLakeFileSystemClient for the file system (container)
                        DataLakeFileSystemClient fileSystemClient = new DataLakeFileSystemClient(new Uri(fileSystemEndpoint), new DefaultAzureCredential());

                        if (!await fileSystemClient.ExistsAsync())
                        {
                            return new JsonResult(new { StatusCode = 400, Message = "File system does not exist." });
                        }

                        // Get a client for the file you want to upload
                        DataLakeFileClient fileClient = fileSystemClient.GetFileClient(fileName);

                        // Open file stream and upload
                        using Stream stream = file.OpenReadStream();
                        await fileClient.UploadAsync(stream, overwrite: true);

                        // Optionally set content type
                        string contentType = GetContentType(fileExtension);
                        await fileClient.SetHttpHeadersAsync(new Azure.Storage.Files.DataLake.Models.PathHttpHeaders { ContentType = contentType });

                        // Set metadata or tags (if needed, Data Lake uses metadata, not blob tags)
                        IDictionary<string, string> metadata = new Dictionary<string, string>
                    {
                        { "file", fileName },
                        { "period", DateTime.Now.ToString("yyyy-MM-dd") }
                    };
                        await fileClient.SetMetadataAsync(metadata);

                        var fileUrl = fileClient.Uri.AbsoluteUri;
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

            return new JsonResult(new { StatusCode = 200, Message = "File uploaded successfully." });
        }

        public async Task<IActionResult> uploadBlob_BLOB(IFormFile file)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, sslPolicyErrors) => true;
                    if (file.Length > 0)
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        var fileExtension = Path.GetExtension(fileName).ToLower(); ;

                        string ContainerName = _configuration["Azure-Blobstorage:ContainerName"];

                        string storageAccountName = _configuration["Azure-Blobstorage:AccountName"];
                        string containerEndpoint = $"https://{storageAccountName}.blob.core.windows.net/{ContainerName}/";

                        BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                                        new DefaultAzureCredential());

                        if (!await containerClient.ExistsAsync())
                        {
                            return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                        }

                        BlobClient blobClient = containerClient.GetBlobClient(fileName);
                        string contentType = GetContentType(fileExtension);
                        using Stream stream = file.OpenReadStream();
                        blobClient.Upload(stream, overwrite: true);
                        //  await blobClient.SetAccessTierAsync(AccessTier.Hot);
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
    }
}
