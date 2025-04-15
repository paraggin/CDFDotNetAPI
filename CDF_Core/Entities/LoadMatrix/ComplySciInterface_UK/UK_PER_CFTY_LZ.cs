using System.ComponentModel.DataAnnotations;

namespace CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK
{
    public class UK_PER_CFTY_LZ
    {
        public UK_PER_CFTY_LZ() { }

        [Key]
        [StringLength(11)]
        public string emplid { get; set; }

        public DateTime? effdt { get; set; }
       
        public int orderbynum { get; set; }
    }
    public class UK_PER_CFTY_LZ_Response
    {
        public List<UK_PER_CFTY_LZ>? Result { get; set; }
        public int Status { get; set; }
    }
}
