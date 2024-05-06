using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.Holiday_Calendar
{
    public class Holiday
    {
        public Holiday() { }
        public int Id { get; set; }
        public string? HolidayDt { get; set; }
        public string? HolidayTitle { get; set; }
        public string? Country { get; set; }
    }
}
