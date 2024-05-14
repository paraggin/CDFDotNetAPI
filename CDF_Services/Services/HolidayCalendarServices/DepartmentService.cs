using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.Services.HolidayCalendarServices
{
    public class DepartmentService : IITDepartmentService
    {
        private readonly ApplicationDBContext _dbContext;

        public DepartmentService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> ListITDepartment()
        {
            var iTDepartmentList = _dbContext.ITDepartment.ToList();

            return new JsonResult(new { data = iTDepartmentList });
        }
    }
}
