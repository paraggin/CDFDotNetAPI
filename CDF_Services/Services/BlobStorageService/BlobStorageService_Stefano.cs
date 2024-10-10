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
using System.Globalization;
using Azure;

namespace CDF_Services.Services.BlobStorageService
{
    public class BlobStorageService_Stefano : IBlobStorageService_Stefano
    {
        private readonly IConfiguration _configuration;

        public BlobStorageService_Stefano(IConfiguration configuration)
        {
         
            _configuration = configuration;
        }
        private string ProcessCSVStream(Stream stream)
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
                return "Failed";
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

        async Task<string> getBLOBSasUrlForJsonFormat(string blobName)
        {
            string sasUrl = "";
            int statusCode = 200; // Default to 200 OK
            try
            {
                string accountName = _configuration["AzureBlobStorageStefano:AccountName"];
                string containerName = _configuration["AzureBlobStorageStefano:ContainerName"];

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




        public async  Task<string> ConvertToJsonFromUrl(string fileName)
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
        public async Task<IActionResult> downloadBlob(string name)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                string accountName = _configuration["AzureBlobStorageStefano:AccountName"];
                string containerName = _configuration["AzureBlobStorageStefano:ContainerName"];

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
                }
                var stream1 = new MemoryStream();

                return new FileContentResult(stream1.ToArray(), "")
                {
                    FileDownloadName = blobClient.Name
                };

            }
        }

        public async Task<IActionResult> FilterBlobs(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {

            try
            {
              //  string connectionString = _configuration["AzureBlobStorageStefano:ConnectionString"];
                string accountName = _configuration["AzureBlobStorageStefano:AccountName"];


                var credential = new DefaultAzureCredential();
                var serviceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), credential);


                containerName = containerName == "" ? _configuration["AzureBlobStorageStefano:ContainerName"] : containerName;
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


        public async  Task<IActionResult> uploadBlob(IFormFile file)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    if (file.Length > 0)
                    {

                        var fileName = Path.GetFileName(file.FileName);
                        var fileExtension = Path.GetExtension(fileName).ToLower(); ;

                        string containerEndpoint = _configuration["AzureBlobStorageStefano:ContainerEndpoint"];

                        // Get a credential and create a client object for the blob container.
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
