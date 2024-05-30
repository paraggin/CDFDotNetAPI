using AutoMapper;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using CDF_Core.Entities.Blob_Storage;
using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Data.Entity;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;


namespace CDF_Services.Services.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IGenericRepository<Blob_Storage> _IGenericRepository;
        private readonly IUnitOfWork<Blob_Storage> _IUnitOfWork;
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public BlobStorageService( IConfiguration configuration,IGenericRepository<Blob_Storage> iGenericRepository, IUnitOfWork<Blob_Storage> iUnitOfWork, ApplicationDBContext dbContext, IMapper mapper)
        {
            _IGenericRepository = iGenericRepository;
            _IUnitOfWork = iUnitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
            _configuration = configuration;
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
      
            public async Task<IActionResult> FilterBlobs(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
        {


            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var serviceClient = new BlobServiceClient(connectionString);

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
                writer.WriteLine("---------------"+DateTime.Now+"---------------");
                writer.WriteLine("Q : "+query);
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



        public async Task<IActionResult> ListBlobsAsyncREST()
        {


          string  storageAccountName = "blobpoc02";
           string storageAccountKey = "a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==";
            string containerName = "container-test";

            try {
                // Construct the URI to list blobs in the container.
                string uri = $"http://{storageAccountName}.blob.core.windows.net/{containerName}?restype=container&comp=list";

                // Instantiate the request message.
                using var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

                // Add the request headers for x-ms-date and x-ms-version.
                DateTime now = DateTime.UtcNow;
                httpRequestMessage.Headers.Add("x-ms-date", now.ToString("R", CultureInfo.InvariantCulture));
                httpRequestMessage.Headers.Add("x-ms-version", "2020-04-08");

                // Add the authorization header.
                httpRequestMessage.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("SharedKey",
                    GetSharedKeyAuthorizationHeader(storageAccountName, storageAccountKey, now, httpRequestMessage));

                // Send the request.
                using var httpClient = new HttpClient();
                using var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, CancellationToken.None);

                return new JsonResult(new { Blobs = httpResponseMessage });


                // If successful (status code = 200), 
                // parse the XML response for the blob names.
                using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                {
                    writer.WriteLine($"Blob STATUS CODE  = {httpResponseMessage.StatusCode}");

                }
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    string xmlString = await httpResponseMessage.Content.ReadAsStringAsync();
                    XElement x = XElement.Parse(xmlString);
                    foreach (XElement blob in x.Element("Blobs").Elements("Blob"))
                    {

                        using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                        {
                            writer.WriteLine("---------------" + DateTime.Now + "---------------");
                            writer.WriteLine($"Blob name = {blob.Element("Name").Value}");

                        }
                    }
                }
            }
            catch(Exception ex)
            {
                using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");
                    writer.WriteLine("BLOB REST API Error : "+ex.Message);

                }
            }
            return null;

        }
    }


}
