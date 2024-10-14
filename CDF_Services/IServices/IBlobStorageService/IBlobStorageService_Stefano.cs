using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IBlobStorageService_Stefano
    {
        Task<IActionResult> uploadBlob(IFormFile file);

        Task<IActionResult> DeleteBlob(string fileName);

        Task<IActionResult> downloadBlob(string name);

        Task<IActionResult> FilterBlobs(int pageSize, int pageNumber);

        Task<string> ConvertToJsonFromUrl(string fileName);


    }
}
