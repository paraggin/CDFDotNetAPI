using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IAzure_BlobstorageService
    {
        Task<IActionResult> uploadBlob(IFormFile file);

        Task<IActionResult> downloadBlob(string name);

        Task<IActionResult> DeleteBlob(string blobName);

        Task<IActionResult> FilterBlobs(int pageSize, int pageNumber);

        Task<string> ConvertToJsonFromUrl(string fileName);

    }
}
