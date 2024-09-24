using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.ILogViewerService
{
    public interface ILogViewerService
    {
        Task<IActionResult> GetAllStorageAccounts();

        Task<IActionResult> GetAllAccountContainer(string storageAccountName);

        Task<IActionResult> GetAllContainerBlob(string storageAccountName, string containerName);

        Task<IActionResult> FilterContainerBlob(string storageAccountName, string containerName, string searchText);

        Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName);

        Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName);

    }
}
