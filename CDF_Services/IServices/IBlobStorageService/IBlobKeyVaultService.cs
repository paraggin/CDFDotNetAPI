using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IBlobKeyVaultService
    {
        Task<IActionResult> GenerateBlobSASUrl(string storageAccountName, string containerName, string blobName);

    }
}
