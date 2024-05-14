using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.IServices.IHolidayCalendarServices
{
    public interface IHolidayService
    {
        Task<IActionResult> ListHolidays();

    }
}
