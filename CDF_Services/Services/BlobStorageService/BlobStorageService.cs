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
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Data.Entity;
using System.Globalization;


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



        public async Task<IActionResult> FilterBlobs1(int pageSize, int pageNumber, string period, string reportingUnit, string filename, string containerName)
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
            if (!string.IsNullOrEmpty(startdate))
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

                writer.WriteLine(query);
            }

            string continuationToken = null;
            int skip = (int)((pageNumber - 1) * pageSize);
            List<Object> blobTags = new List<Object>();

            if (!string.IsNullOrEmpty(query))
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


       }


}
