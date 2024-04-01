using AutoMapper;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Core.Entities.Blob_Storage;
using CDF_Core.Entities.PNP_Accounts;
using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Mvc;


namespace CDF_Services.Services.BlobStorageService
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly IGenericRepository<Blob_Storage> _IGenericRepository;
        private readonly IUnitOfWork<Blob_Storage> _IUnitOfWork;
        private readonly ApplicationDBContext _dbContext;
        private readonly IMapper _mapper;

        private const string _connectionString = "DefaultEndpointsProtocol=https;AccountName=blobpoc02;AccountKey=a8NjedNF5CBZ76krN/HDW5QXdDR8lapH1Pqh8flb1imX5MrsN3ZVv44BciaB9XTQK2mhtTHanvGK+AStqO7PFg==;EndpointSuffix=core.windows.net";

        public BlobStorageService(IGenericRepository<Blob_Storage> iGenericRepository, IUnitOfWork<Blob_Storage> iUnitOfWork, ApplicationDBContext dbContext, IMapper mapper)
        {
            _IGenericRepository = iGenericRepository;
            _IUnitOfWork = iUnitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<IActionResult> ListBlobs(string container_name, int pageSize, int pageNumber)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container_name);

            int totalCount = 0;
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
            {
                totalCount++;
            }
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

        /*  public async Task<IActionResult> ListBlobs(string container_name, int pageSize, int pageNumber)
          {
              BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
              BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container_name);

              string continuationToken = null;
              List<BlobItem> blobs = new List<BlobItem>();

              await foreach (Page<BlobItem> page in
                  containerClient.GetBlobsAsync(traits: BlobTraits.Metadata, prefix: null).AsPages(continuationToken, pageSize))
              {

                  Console.WriteLine($"Page: {page}");
                  foreach (BlobItem blobItem in page.Values)
                  {

                      Console.WriteLine($"Blob Name: {blobItem.Name}");
                      blobs.Add(blobItem);
                  }
                  break;
                  continuationToken = page.ContinuationToken;
              }

              return new JsonResult(blobs);
          }*/

        /*   public async Task<IActionResult> ListBlobs(string container_name, int pageSize, int pageNumber)
           {
               var blobServiceClient = new BlobServiceClient(_connectionString);
               var containerClient = blobServiceClient.GetBlobContainerClient(container_name);

               var blobs = new List<Blob_Storage>();
               var blobsToSkip = (pageNumber - 1) * pageSize;
               var blobsSkipped = 0;

               await foreach (var blobItem in containerClient.GetBlobsAsync())
               {
                   if (blobsSkipped < blobsToSkip)
                   {
                       blobsSkipped++;
                       continue;
                   }

                   var blobClient = containerClient.GetBlobClient(blobItem.Name);
                   var blobProperties = await blobClient.GetPropertiesAsync();

                   var metadata = new Blob_Storage
                   {
                       Name = blobItem.Name,
                       Version = blobProperties.Value.Metadata.ContainsKey("Version") ? blobProperties.Value.Metadata["Version"] : null,
                       CreatedDate = blobProperties.Value.Metadata.ContainsKey("CreatedDate") ? blobProperties.Value.Metadata["CreatedDate"] : null,
                       ModifiedDate = blobProperties.Value.Metadata.ContainsKey("ModifiedDate") ? blobProperties.Value.Metadata["ModifiedDate"] : null
                   };

                   blobs.Add(metadata);

                   if (blobs.Count >= pageSize)
                   {
                       break;
                   }
               }

               return new JsonResult(blobs);
           }
   */
    }


}
