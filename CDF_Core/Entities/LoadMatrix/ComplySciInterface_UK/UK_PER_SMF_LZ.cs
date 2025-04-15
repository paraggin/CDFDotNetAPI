using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK
{
    public class UK_PER_SMF_LZ
    {
        public UK_PER_SMF_LZ() { }

        [Key]
        
        [StringLength(11)]
        public string emplid { get; set; }       

        public DateTime? effdt { get; set; }

        [StringLength(4)]
        public string uk_smcr_smf_lz { get; set; }
    }

    public class UK_PER_SMF_LZ_Response
    {
        public List<UK_PER_SMF_LZ>? Result { get; set; }
        public int Status { get; set; }
    }
}
