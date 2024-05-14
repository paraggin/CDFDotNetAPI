namespace CDF_Core.Entities.Holiday_Calendar
{
    public class WorkForceMaster
    {

        public WorkForceMaster() { }
        public int Id { get; set; }

        public string Employee { get; set; }
      
        public string Department { get; set; }

        public int SupervisorEmplId { get; set; }
      
        public string OpSysId { get; set; }
     
        public string Country { get; set; }
    }
}
