using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

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


        [HttpGet]
        [Route("DownloadFile")]
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
        public async Task<IActionResult> ListBlobs(string container_name, int pageSize, int pageNumber)
        {
           
           return await _BlobStorageService.ListBlobs(container_name, pageSize, pageNumber+1);
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

            List<string> searchConditions = new List<string>();
            searchConditions.Add($"\"Version\" = '{searchText}'");
            // searchConditions.Add($"\"CreatedDate\" = '{searchText}'");
            //searchConditions.Add($"\"modifiedDate\" = '{searchText}'");


            // string query =  string.Join(" | ", searchConditions) ;
            string query = $"\"Version\" = '{searchText}'";

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
