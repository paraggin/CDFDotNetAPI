using CDF_Core.Entities.SnowFlake;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.ISnowFlakeService
{
    public interface ISnowFlakeService
    {

         Task<IActionResult> ListEmployeeDetail();
    }
}
