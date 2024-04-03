using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.IO;

using Constants = CDF_Services.Constants.Constants;
using Azure.Storage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;

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


        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile(string container_name, string prefix)
        {
            BlobClient blobClient = new BlobClient(_connectionString, container_name, prefix);
            using (var stream = new MemoryStream())
            {
               var temp= await blobClient.DownloadToAsync(stream);
                stream.Position = 0;
                var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
                return File(stream.ToArray(), contentType, blobClient.Name);
            }
        }

        [HttpGet]
        [Route("ListBlobs")]
        public async Task<IActionResult> ListBlobs(string container_name, int pageSize, int pageNumber)
        {
           
           return await _BlobStorageService.ListBlobs(container_name, pageSize, pageNumber+1);
        }

        [HttpGet]
        [Route("getBLobSAS")]
        public async Task<IActionResult> getBLobSAS(string BlobName)
        {

            const string AccountName = "blobpoc02";
            const string AccountKey = "a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==";
            const string ContainerName = "container-poc";
            const string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=blobpoc02;AccountKey=a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==;EndpointSuffix=core.windows.net";
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                BlobContainerClient blobContainerClient = new BlobContainerClient(ConnectionString,
                ContainerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(BlobName);

                Azure.Storage.Sas.BlobSasBuilder blobSasBuilder = new Azure.Storage.Sas.BlobSasBuilder()
                {
                    BlobContainerName = ContainerName,
                    BlobName =BlobName,
                    ExpiresOn = DateTime.UtcNow.AddMinutes(15),
                };
                blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
                var sasToken = blobSasBuilder.ToSasQueryParameters(new
                StorageSharedKeyCredential(AccountName, AccountKey)).ToString();
                var sasURL = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";
                writer.WriteLine(sasURL);
                return new JsonResult(new { blobSASUrl = sasURL });

            }

        }

        [HttpGet]
        [Route("ListBlobsv3")]
        public async Task<IActionResult> ListBlobsv3(int pageSize, int pageNumber, string? searchText)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=blobpoc02;AccountKey=a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==;EndpointSuffix=core.windows.net";

            var serviceClient = new BlobServiceClient(connectionString);

            string containerName = "container-poc";
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);

            int totalCount = 0;
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
            {
                totalCount++;
            }
            List<TaggedBlobItem> blobs = new List<TaggedBlobItem>();

            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {

                //   string query = $"\"Version\" = '{searchText}' | \"CreatedDate\" = '{searchText}' | \"ModifiedDate\" = '{searchText}'  | \"createdDate\" = '{searchText}' | \"modifiedDate\" = '{searchText}' | \"version\" = '{searchText}'";
                string query = $"\"Version\" = '{searchText}' | \"CreatedDate\" = '{searchText}' | \"ModifiedDate\" = '{searchText}'  | \"createdDate\" = '{searchText}' | \"modifiedDate\" = '{searchText}' | \"version\" = '{searchText}'";

                writer.WriteLine($"Query: {query}");

                string continuationToken = null;
                int skip = (pageNumber - 1) * pageSize;
                await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query)
                    .AsPages(continuationToken, pageSize))
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

                        BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                        writer.WriteLine($"Properties: {blobProperties.ToString()}");

                        writer.WriteLine($"Properties: {blobProperties.Metadata}");
                        writer.WriteLine($"Properties: {blobProperties.TagCount}");

                        /* writer.WriteLine($"  Content Type: {blobProperties.ContentType}");
                         writer.WriteLine($"  Content Length: {blobProperties.ContentLength}");
                         writer.WriteLine($"  Last Modified: {blobProperties.LastModified}");*/
                        writer.WriteLine();


                    }

                    if (blobs.Count >= pageSize)
                    {
                        break;
                    }

                    skip = Math.Max(0, pageSize - blobs.Count);
                    continuationToken = page.ContinuationToken;
                }
            }
            return new JsonResult(new { TotalCount = totalCount, Blobs = blobs });
        }

       [HttpGet]
        [Route("ListBlobsv2")]
        public async Task<IActionResult> FilterBlobs( int pageSize, int pageNumber,string? searchText)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=blobpoc02;AccountKey=a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==;EndpointSuffix=core.windows.net";

            var serviceClient = new BlobServiceClient(connectionString);

            string containerName = "container-poc";
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            int totalCount = 0;
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
            {
                totalCount++;
            }
            List<TaggedBlobItem> filteredBlobs = new List<TaggedBlobItem>();
            List<string> filteredBlobsName = new List<string>();
            //createdDate
            //modifiedDate
            //Version

            /*   List<string> searchConditions = new List<string>();
               searchConditions.Add($"\"Version\" = '{searchText}'");          
               string query = $"\"Version\" = '{searchText}' | \"CreatedDate\" = '{searchText}' | \"ModifiedDate\" = '{searchText}'  | \"createdDate\" = '{searchText}' | \"modifiedDate\" = '{searchText}' | \"version\" = '{searchText}'";
   */

            List<(string field, string value)> searchConditions = new List<(string field, string value)>();
            searchConditions.Add(("Version", searchText));
            searchConditions.Add(("CreatedDate", searchText));
            searchConditions.Add(("ModifiedDate", searchText));
            searchConditions.Add(("createdDate", searchText));
            searchConditions.Add(("modifiedDate", searchText));
            searchConditions.Add(("version", searchText));

            searchConditions.Sort((x, y) => x.field.CompareTo(y.field));

            List<string> queryConditions = new List<string>();
            foreach (var condition in searchConditions)
            {
                queryConditions.Add($"\"{condition.field}\" = '{condition.value}'");
            }

            string query = string.Join(" | ", queryConditions);
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            { 
                writer.WriteLine(query);
            }

                List<TaggedBlobItem> blobs = new List<TaggedBlobItem>();

            string continuationToken = null;
            int skip = (pageNumber - 1) * pageSize;
            await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query)
                .AsPages(continuationToken, pageSize))
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

                    // Get specific details of the blob
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                    await blobClient.GetTagsAsync();
                    blobs.Add(blobItem);
                }
                if (blobs.Count >= pageSize)
                {
                    break;
                }
                skip = Math.Max(0, pageSize - blobs.Count);

                continuationToken = page.ContinuationToken;
            }

            return new JsonResult(new { TotalCount = totalCount, Blobs = blobs });
            /*   if (!string.IsNullOrEmpty(searchText))
               {
                   searchConditions.Add($"\"Version\" = '{searchText}'");
                   // searchConditions.Add($"\"CreatedDate\" = '{searchText}'");
                   //searchConditions.Add($"\"modifiedDate\" = '{searchText}'");


                   string query = searchConditions.Count > 0 ? string.Join(" | ", searchConditions) : null;

                   List<TaggedBlobItem> blobs = new List<TaggedBlobItem>();

                   string continuationToken = null;
                   int skip = (pageNumber - 1) * pageSize;
                   await foreach (Page<TaggedBlobItem> page in containerClient.FindBlobsByTagsAsync(query)
                       .AsPages(continuationToken, pageSize))
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

                           // Get specific details of the blob
                           BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                           await blobClient.GetTagsAsync();
                           blobs.Add(blobItem);
                       }
                       if (blobs.Count >= pageSize)
                       {
                           break;
                       }
                       skip = Math.Max(0, pageSize - blobs.Count);

                       continuationToken = page.ContinuationToken;
                   }

                   return new JsonResult(new { TotalCount = totalCount, Blobs = blobs });


               }
               else
               {

                   BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);


                   int skip = (pageNumber - 1) * pageSize;

                   string continuationToken = null;
                   List<BlobItem> blobs = new List<BlobItem>();

                   await foreach (Page<BlobItem> page in containerClient.GetBlobsAsync(traits: BlobTraits.Metadata, prefix: null)
                       .AsPages(continuationToken, pageSize))
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
                           Console.WriteLine($"Blob Name: {blobItem.Name}");
                           blobs.Add(blobItem);
                       }
                       if (blobs.Count >= pageSize)
                       {
                           break;
                       }
                       skip = Math.Max(0, pageSize - blobs.Count);

                       continuationToken = page.ContinuationToken;
                   }

                   return new JsonResult(new { TotalCount = totalCount, Blobs = blobs });
               }

   */
        }


    }
}
