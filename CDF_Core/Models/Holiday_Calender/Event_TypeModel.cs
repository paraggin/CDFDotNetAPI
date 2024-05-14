using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Holiday_Calender
{
    public class Event_TypeModel
    {
        public int Id { get; set; }

        public string EventCode { get; set; }

        public string EventDescr { get; set; }

        public char IsWorking { get; set; }

        public string GenDescr { get; set; }

        public char Status { get; set; }

    }
}
