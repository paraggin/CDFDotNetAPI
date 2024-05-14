using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.Services.HolidayCalendarServices
{
    public class WorkForceMasterService : IWorkForceMasterService
    {
        private readonly ApplicationDBContext _dbContext;

        public WorkForceMasterService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> ListWorkForceMaster()
        {

            var workForceList = _dbContext.WorkForceMaster.ToList();

            return new JsonResult(new { data = workForceList });
        }

       
    }
}
