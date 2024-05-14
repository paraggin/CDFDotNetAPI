

namespace CDF_Core.Entities.Holiday_Calendar
{
    public class ITDepartment
    {
      
        public ITDepartment() { }
        public int Id { get; set; }
        
        public string CostCenter { get; set; }
      
        public string CostCenterDescr { get; set; }

        public int BusinessUnit { get; set; }

        public int DeptId { get; set; }
       
        public string DeptDescr { get; set; }
    }
}
