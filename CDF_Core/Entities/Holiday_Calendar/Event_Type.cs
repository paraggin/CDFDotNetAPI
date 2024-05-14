using System.ComponentModel.DataAnnotations;

namespace CDF_Core.Entities.Holiday_Calendar
{
    public class Event_Type
    {
      
        public Event_Type() { }
        public int Id { get; set; }

        public string EventCode { get; set; }

        public string EventDescr { get; set; }

        public char IsWorking { get; set; }
   
        public string GenDescr { get; set; }

        public char Status { get; set; }

    }
}
