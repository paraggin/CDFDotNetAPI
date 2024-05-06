using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.Holiday_Calendar
{
    public class Event
    {
        public Event() { }
        public int Id { get; set; }
        public string? Summary { get; set; }
        public string? StartDate { get; set; }
        public string? AllDayEvent { get; set; }
        public string? EventDescr { get; set; }
        public string? Comment { get; set; }
        public string? CreatedDate { get; set; }
        public string? ModifiedDate { get; set; }
    }
}
