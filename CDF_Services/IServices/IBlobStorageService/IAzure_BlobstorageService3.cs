using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IAzure_BlobstorageService3
    {

        Task<IActionResult> uploadBlob(IFormFile file);

        Task<IActionResult> downloadBlob(string name);

        Task<IActionResult> DeleteBlob(string blobName);

        Task<IActionResult> FilterBlobs(int pageSize, int pageNumber);

        Task<string> ConvertToJsonFromUrl(string fileName);
    }
}
