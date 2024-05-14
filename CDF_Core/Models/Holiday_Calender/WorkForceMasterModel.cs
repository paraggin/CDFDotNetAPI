namespace CDF_Core.Models.Holiday_Calender
{
    public class WorkForceMasterModel
    {
        public int Id { get; set; }

        public string Employee { get; set; }

        public string Department { get; set; }

        public int SupervisorEmplId { get; set; }

        public string OpSysId { get; set; }

        public string Country { get; set; }
    }
}
