using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK
{
    public class CSC_INST_GRP_LZ
    {
        [Key]
        [StringLength(5)]
        public string setid { get; set; }

        [StringLength(50)]
        public string csc_group_lz { get; set; }

        public DateTime? effdt { get; set; }

        [StringLength(1)]
        public string eff_status { get; set; }

        [StringLength(30)]
        public string descr { get; set; }

        [StringLength(3)]
        public string csc_grp_typ_fl_lz { get; set; }

        [StringLength(30)]
        public string rolename { get; set; }

        public byte[] audit_sbr_lz { get; set; }

        public DateTime? createdttm { get; set; }

        [StringLength(30)]
        public string createoprid { get; set; }

        public DateTime? lastupddttm { get; set; }

        [StringLength(30)]
        public string lastupdoprid { get; set; }
    }

    public class CSC_INST_GRP_LZResponse
    {
        public List<CSC_INST_GRP_LZ>? Result { get; set; }
        public int Status { get; set; }
    }
}
