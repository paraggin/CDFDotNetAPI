using CDF_Core.Entities.PNP_Accounts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.IBlobStorageService
{
    public interface IBlobStorageService
    {
        
        Task<IActionResult> getBLobSAS(string BlobName);

        Task<IActionResult> FilterBlobs(int pageSize, int pageNumber, string startdate, string enddate, string period, string reportingUnit, string filename);

    }
}
