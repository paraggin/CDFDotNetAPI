using CDF_Core.Entities;

namespace CDF_Core.Models.Holiday_Calender
{
    public class EventModel 
    {
        public int Id { get; set; }
        public string? Summary { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? AllDayEvent { get; set; }
        public string? EventDescr { get; set; }
        public string? Comment { get; set; }
        public string? CreatedDate { get; set; }
        public string? ModifiedDate { get; set; }
    }
}
 