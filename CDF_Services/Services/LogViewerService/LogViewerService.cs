using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using Microsoft.AspNetCore.Mvc;
using CDF_Services.IServices.ILogViewerService;
using Microsoft.Extensions.Configuration;
using Azure.ResourceManager.Storage;


namespace CDF_Services.Services.LogViewerService
{
    public  class LogViewerService : ILogViewerService
    {

        private readonly IConfiguration _configuration;
        public LogViewerService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IActionResult> GetAllStorageAccounts()
        {
            var storageAccountList = new List<object>();
            try
            {
                var subscriptionId = _configuration["AzureBlobStorage:SubscriptionId"];

                ArmClient armClient = new ArmClient(new DefaultAzureCredential());

                SubscriptionResource subscription = armClient.GetSubscriptionResource(new ResourceIdentifier($"/subscriptions/{subscriptionId}"));

                await foreach (var storageAccount in subscription.GetStorageAccountsAsync())
                {
                    storageAccountList.Add(new { name = storageAccount.Data.Name });
                }

                return new JsonResult(new { StatusCode = 200, data = storageAccountList });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { StatusCode = 400, message = ex.Message });
            }
        }
        public async Task<IActionResult> GetAllAccountContainer(string storageAccountName)
        {
            var containerList = new List<object>();
            try
            {
                var subscriptionId = _configuration["AzureBlobStorage:SubscriptionId"];

                string storageAccountUrl = $"https://{storageAccountName}.blob.core.windows.net";

                var blobServiceClient = new BlobServiceClient(new Uri(storageAccountUrl), new DefaultAzureCredential());

                await foreach (BlobContainerItem container in blobServiceClient.GetBlobContainersAsync())
                {
                    containerList.Add(new { name = container.Name });
                }

                return new JsonResult(new { StatusCode = 200, data = containerList });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return new JsonResult(new { StatusCode = 400, message = ex.Message });
            }
        }

        public async Task<IActionResult> DeleteBlob(string storageAccountName,string ContainerName, string fileName)
        {
            using (StreamWriter writer = System.IO.File.AppendText("log.txt"))
            {
                try
                {
                    if (string.IsNullOrEmpty(fileName))
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "File name cannot be null or empty." });
                    }
                  //  
                   // string ContainerName = _configuration["AzureBlobStorage:ContainerName"];

                  //  string storageAccountName = _configuration["AzureBlobStorage:AccountName"];
                    //  string containerEndpoint = _configuration["AzureBlobStorage:ContainerEndpoint"];
                    string containerEndpoint = $"https://{storageAccountName}.blob.core.windows.net/{ContainerName}/";


                    // Get a credential and create a client object for the blob container.
                    BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint),
                                                                                   new DefaultAzureCredential());

                    // Check if the container exists
                    if (!await containerClient.ExistsAsync())
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "Container does not exist." });
                    }

                    // Get a reference to the blob
                    BlobClient blobClient = containerClient.GetBlobClient(fileName);

                    // Delete the blob if it exists
                    var deleteResponse = await blobClient.DeleteIfExistsAsync();

                    if (deleteResponse.Value)
                    {
                        writer.WriteLine("---------------" + DateTime.Now + "---------------");
                        writer.WriteLine($"Blob {fileName} deleted successfully.");
                    }
                    else
                    {
                        return new JsonResult(new { StatusCode = 400, Message = "Blob not found or already deleted." });
                    }
                }
                catch (Exception ex)
                {
                    writer.WriteLine("---------------" + DateTime.Now + "---------------");
                    writer.WriteLine(ex.Message);
                    return new JsonResult(new { StatusCode = 400, Message = ex.Message });
                }
            }

            return new JsonResult(new { StatusCode = 200, Message = "Blob deleted successfully." });
        }

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
                            name = blobItem.Name
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
      
        public async Task<IActionResult> GetBlobSASUrlUsingAcAccessKey(string accountName, string containerName, string blobName)
        {
            try
            {

                string sasUrl = string.Empty;
                string accountKey = _configuration["AzureBlobStorage:AccountKey"];

                var storageSharedKeyCredential = new StorageSharedKeyCredential(accountName, accountKey);

                BlobServiceClient blobServiceClient = new BlobServiceClient(new Uri($"https://{accountName}.blob.core.windows.net"), storageSharedKeyCredential);
                BlobContainerClient blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

                BlobSasBuilder blobSasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobContainerClient.Name,
                    BlobName = blobClient.Name,
                    StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
                    ExpiresOn = DateTime.UtcNow.AddHours(24),
                    Protocol = SasProtocol.Https,
                    Resource = "b" // 'b' is for Blob
                };

                blobSasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Delete);

                string sasToken = blobSasBuilder.ToSasQueryParameters(storageSharedKeyCredential).ToString();

                sasUrl = $"{blobClient.Uri.AbsoluteUri}?{sasToken}";
                return new JsonResult(new { StatusCode = 200, Url = $"{blobClient.Uri}?{sasToken}" });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating SAS URL: {ex.Message}");
                return new JsonResult(new { StatusCode = 400, Error = ex.Message });

            }

        }

    }
}
