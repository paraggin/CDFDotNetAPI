using AutoMapper;
using Azure.Storage.Blobs;
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

        public async Task<IActionResult> ListBlobs(string container_name)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(container_name);

            var blobs = new List<Blob_Storage>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
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
            }

            return new JsonResult(blobs);
        }
        public async Task<List<string>> ListBlobUrls(string containerName)
        {
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            var blobUrls = new List<string>();
            await foreach (var blobItem in containerClient.GetBlobsAsync())
            {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                var blobUrl = blobClient.Uri.ToString();
                blobUrls.Add(blobUrl);
            }

            return blobUrls;
        }
    }


}
