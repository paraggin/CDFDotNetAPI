using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Services.HolidayCalendarServices
{
    public class Event_TypeService : IEvent_TypeService
    {
        private readonly ApplicationDBContext _dbContext;
        public Event_TypeService(ApplicationDBContext dbContext) {
            _dbContext = dbContext;
                    }

        public async Task<IActionResult> ListEvent_TypeList()
        {
            var event_TypeList = _dbContext.Event_Type.ToList();

            return new JsonResult(new { data = event_TypeList });
        }
    }
}
