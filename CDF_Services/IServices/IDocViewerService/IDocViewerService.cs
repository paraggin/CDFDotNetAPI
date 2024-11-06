using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.IDocViewerService
{
    public interface IDocViewerService
    {

        Task<IActionResult> GetAllContainerBlob(string storageAccountName, string containerName);

        Task<IActionResult> FilterContainerBlob(string storageAccountName, string containerName, string searchText);

        Task<IActionResult> GetBlobSasUrl(string storageAccountName, string containerName, string blobName);

        Task<IActionResult> downloadBlob(string storageAccountName, string containerName, string blobName);

    }
}
