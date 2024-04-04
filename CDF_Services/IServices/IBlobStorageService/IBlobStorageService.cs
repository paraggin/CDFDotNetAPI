using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IBlobStorageService
    {
        
        Task<IActionResult> getBLobSAS(string BlobName);

        Task<IActionResult> FilterBlobs(int pageSize, int pageNumber, string period, string reportingUnit, string filename,string containerName);


    }
}
