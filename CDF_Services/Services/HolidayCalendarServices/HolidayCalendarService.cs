using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDF_Core.Entities.Holiday_Calendar;
using CDF_Core.Entities.PNP_Accounts;
using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IHolidayCalendarServices;
using Microsoft.AspNetCore.Mvc;

namespace CDF_Services.Services.HolidayCalendarServices
{
    public class HolidayCalendarService : IHolidayCalendarService
    {
        private readonly ApplicationDBContext _dbContext;
        private readonly IGenericRepository<Event> _IGenericRepository;
        private readonly IUnitOfWork<Event> _IUnitOfWork;


        public HolidayCalendarService(ApplicationDBContext dbContext) {
            _dbContext = dbContext;
        }
        public async Task<IActionResult> ListEvents()
        {

            var eventList = _dbContext.Event.ToList();


            return new JsonResult(new { data = eventList });
        }

        public async Task<IActionResult> ListHolidays()
        {

            var holidayList = _dbContext.Holiday.ToList();

            return new JsonResult(new { data = holidayList });
        }
    }
}
