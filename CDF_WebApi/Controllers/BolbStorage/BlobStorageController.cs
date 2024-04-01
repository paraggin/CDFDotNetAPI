using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

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
        public async Task<IActionResult> FilterBlobs( int pageSize, int pageNumber,string searchText)
        {
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=blobpoc02;AccountKey=a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==;EndpointSuffix=core.windows.net";

            var serviceClient = new BlobServiceClient(connectionString);

            string containerName = "container-poc";
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);

            List<TaggedBlobItem> filteredBlobs = new List<TaggedBlobItem>();
            List<string> filteredBlobsName = new List<string>();
            //createdDate
            //modifiedDate
            //Version
            //string query = " \"Version\" = '0.0.3' ";

            List<string> searchConditions = new List<string>();

            if (!string.IsNullOrEmpty(searchText))
            {
               // searchConditions.Add($"\"Version\" = '{searchText}'");
               // searchConditions.Add($"\"CreatedDate\" = '{searchText}'");
                searchConditions.Add($"\"modifiedDate\" = '{searchText}'");
                // Add more conditions as needed
            }

            // Combine search conditions into a single query
            string query = string.Join(" | ", searchConditions);

            List<TaggedBlobItem> blobs = new List<TaggedBlobItem>();
            /*    await foreach (TaggedBlobItem taggedBlobItem in serviceClient.FindBlobsByTagsAsync(query).AsPages(2))
                {
                    string blobName = taggedBlobItem.BlobName;
                    filteredBlobsName.Add(blobName);
                    blobs.Add(taggedBlobItem);
                }*/


            // List<BlobItem> blobs = new List<BlobItem>();
            string continuationToken = null;
            int skip = (pageNumber - 1) * pageSize;
            await foreach (Page<TaggedBlobItem> page  in containerClient.FindBlobsByTagsAsync(query)
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
                    //Console.WriteLine($"Blob Name: {blobItem.Name}");
                    blobs.Add(blobItem);
                }
                if (blobs.Count >= pageSize)
                {
                    break;
                }
                skip = Math.Max(0, pageSize - blobs.Count);

                continuationToken = page.ContinuationToken;
            }

            return new JsonResult(new { Blobs = blobs });

        }


    }
}
