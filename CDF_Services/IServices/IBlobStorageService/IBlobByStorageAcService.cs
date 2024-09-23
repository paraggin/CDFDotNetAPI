using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IBlobByStorageAcService
    {

        Task<IActionResult> GetAllStorageAccounts();

        Task<IActionResult> GetAllAccountContainer(string storageAccountName);

        Task<IActionResult> GetAllContainerBlob(string storageAccountName,string containerName);

        Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName);

        Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName);

        Task<IActionResult> GetSASUrl(string storageAccountName, string containerName, string blobName);

    }
}
