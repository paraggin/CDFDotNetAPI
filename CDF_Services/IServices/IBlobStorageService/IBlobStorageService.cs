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
        Task<IActionResult> ListBlobs(string container_name);
    }
}
