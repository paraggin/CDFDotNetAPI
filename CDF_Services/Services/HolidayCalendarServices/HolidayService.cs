using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.Services.HolidayCalendarServices
{
    public class HolidayService : IHolidayService
    {
        private readonly ApplicationDBContext _dbContext;

        public HolidayService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> ListHolidays()
        {

            var holidayList = _dbContext.Holiday.ToList();

            return new JsonResult(new { data = holidayList });
        }
    }
}
