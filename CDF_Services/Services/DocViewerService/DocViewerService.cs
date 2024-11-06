using Azure.Identity;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using CDF_Services.IServices.IDocViewerService;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.Services.DocViewerService
{
    public  class DocViewerService : IDocViewerService
    {

        public async Task<IActionResult> GetAllContainerBlob(string storageAccountName, string containerName)
        {
            var blobList = new List<object>();
            try
            {
                string storageAccountUrl = $"https://{storageAccountName}.blob.core.windows.net";

                var blobServiceClient = new BlobServiceClient(new Uri(storageAccountUrl), new DefaultAzureCredential());

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
                {
                    blobList.Add(new
                    {
                        name = blobItem.Name,
                        containerName = containerName
                        //, properties = blobItem.Properties
                    });
                }

                return new JsonResult(new { StatusCode = 200, data = blobList });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { StatusCode = 400, message = ex.Message });
            }
        }


        public async Task<IActionResult> FilterContainerBlob(string storageAccountName, string containerName, string searchText)
        {
            var blobList = new List<object>();
            try
            {
                string storageAccountUrl = $"https://{storageAccountName}.blob.core.windows.net";

                var blobServiceClient = new BlobServiceClient(new Uri(storageAccountUrl), new DefaultAzureCredential());

                var blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
                {
                    if (blobItem.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                    {
                        blobList.Add(new
                        {
                            name = blobItem.Name,
                            containerName = containerName

                            //, properties = blobItem.Properties
                        });
                    }

                }

                return new JsonResult(new { StatusCode = 200, data = blobList });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { StatusCode = 400, message = ex.Message });
            }
        }



        public async Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                string containerEndpoint = $"https://{storageAccountName}.blob.core.windows.net/{containerName}/";

                BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());

                BlobClient blobClient = containerClient.GetBlobClient(blobName);


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
                    writer.WriteLine("Download Error 1  : " + ex.Message);
                }
                var stream1 = new MemoryStream();

                return new FileContentResult(stream1.ToArray(), "")
                {
                    FileDownloadName = blobClient.Name
                };

            }

        }
        public async Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net"), new DefaultAzureCredential());

                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);

                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    BlobName = blobClient.Name,
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                    ExpiresOn = DateTime.UtcNow.AddMinutes(15),
                    Protocol = SasProtocol.Https,
                    Resource = "b"
                };

                blobSasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Delete);

                UserDelegationKey userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddHours(1));

                string sasToken = blobSasBuilder.ToSasQueryParameters(userDelegationKey, storageAccountName).ToString();
                string sasUrl = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";

                return new JsonResult(new { StatusCode = 200, Url = sasUrl });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { StatusCode = 400, message = ex.Message });
            }

        }

    }
}
