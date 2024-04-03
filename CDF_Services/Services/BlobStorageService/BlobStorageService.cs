using AutoMapper;
using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using CDF_Core.Entities.Blob_Storage;
using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;


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
                };
                blobSasBuilder.SetPermissions(Azure.Storage.Sas.BlobSasPermissions.Read);
                var sasToken = blobSasBuilder.ToSasQueryParameters(new
                StorageSharedKeyCredential(AccountName, AccountKey)).ToString();
                var sasURL = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";
                writer.WriteLine(sasURL);
                return new JsonResult(new { blobSASUrl = sasURL });

            }

        }



        public async Task<IActionResult> FilterBlobs(int pageSize , int pageNumber , string startdate, string enddate, string period , string reportingUnit, string filename )
        {
            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];

            var serviceClient = new BlobServiceClient(connectionString);

            string containerName = _configuration["AzureBlobStorage:ContainerName"];
            BlobContainerClient containerClient = serviceClient.GetBlobContainerClient(containerName);
            int totalCount = 0;
            await foreach (BlobItem blob in containerClient.GetBlobsAsync(traits: BlobTraits.None))
            {
                totalCount++;
            }
            string query = "";


            if (!string.IsNullOrEmpty(startdate))
            {
                query += @$"""startdate"" >= '{startdate}'";
            }

            if (!string.IsNullOrEmpty(enddate))
            {
                if (!string.IsNullOrEmpty(query))
                    query += " AND ";

                query += @$"""enddate"" <= '{enddate}'";
            }

            if (!string.IsNullOrEmpty(period))
            {
                if (!string.IsNullOrEmpty(query))
                    query += " AND ";

                query += @$"""period"" = '{period}'";
            }

            if (!string.IsNullOrEmpty(reportingUnit))
            {
                if (!string.IsNullOrEmpty(query))
                    query += " AND ";

                query += @$"""reportingunit"" = '{reportingUnit}'";
            }



            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                writer.WriteLine(query);
            }
            string continuationToken = null;

            int skip = (int)((pageNumber - 1) * pageSize);

            List<Object> blobTags = new List<Object>();

            if (!string.IsNullOrEmpty(query))
            {

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
                return new JsonResult(new { TotalCount = totalCount, Blobs = blobTags });

            }


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


        }


}
