using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using CDF_Services.IServices.IBlobStorageService;
using Microsoft.AspNetCore.Mvc;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using CDF_Core.Entities.SnowFlake;
using Amazon.Runtime.Credentials.Internal;


namespace CDF_Services.Services.BlobStorageService
{
    public class BlobKeyVaultService : IBlobKeyVaultService
    {
        private readonly IConfiguration _configuration;

        public BlobKeyVaultService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<IActionResult> GenerateBlobSASUrl(string storageAccountName, string containerName, string blobName)
        {
            string storageAccountKey = "";
            var sasToken = "";
            try
            {

                string keyVaultUri = _configuration["KeyVault:KeyVaultURI"];
                string storageAccountKeySecretNameTemplate = _configuration["KeyVault:AccountKeySecretNameTemplate"];

                var client = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

                string   storageAccountKeySecretName = string.Format(storageAccountKeySecretNameTemplate, storageAccountName);     
                KeyVaultSecret secret = await client.GetSecretAsync(storageAccountKeySecretName);
                storageAccountKey = secret.Value;


                var storageCredentials = new StorageSharedKeyCredential(storageAccountName, storageAccountKey);

                var blobServiceClient = new BlobServiceClient(new Uri($"https://{storageAccountName}.blob.core.windows.net"), storageCredentials);

                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = containerClient.GetBlobClient(blobName);

                 sasToken = GenerateSasToken(containerClient, blobClient, storageCredentials);

                return new JsonResult(new { StatusCode = 200, Url = $"{blobClient.Uri}?{sasToken}"});
            }
            catch (Exception ex)
            {
                return new JsonResult(new { StatusCode = 400, Error = ex.Message });

            }


        }

        private static string GenerateSasToken(BlobContainerClient containerClient, BlobClient blobClient, StorageSharedKeyCredential storageCredentials)
        {
            // Define SAS token parameters
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b", // "b" for blob, "c" for container
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1) // SAS token valid for 1 hour
            };

            // Set the permissions for the SAS token (e.g., read permission)
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            // Generate the SAS token using the shared key credential
            string sasToken = sasBuilder.ToSasQueryParameters(storageCredentials).ToString();

            return sasToken;
        }
    }
}
