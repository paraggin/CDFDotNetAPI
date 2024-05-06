using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Holiday_Calender
{
    public class HolidayModel
    {
        public int Id { get; set; }
        public string? HolidayDt { get; set; }
        public string? HolidayTitle { get; set; }
        public string? Country { get; set; }
    }
}
