using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.IHolidayCalendarServices
{
    public interface IEvent_TypeService
    {
        Task<IActionResult> ListEvent_TypeList();
    }
}
