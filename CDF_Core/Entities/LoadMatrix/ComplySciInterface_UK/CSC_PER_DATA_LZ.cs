using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK
{
    public class CSC_PER_DATA_LZ
    {
        public CSC_PER_DATA_LZ() { }

        [Key]
        public string setid { get; set; }
        public string emplid { get; set; }
        public int empl_rcd { get; set; }
        public string csc_user_id_lz { get; set; }
        public DateTime? csc_hire_dt_lz { get; set; }
        public DateTime? csc_term_dt_lz { get; set; }
        public string csc_upd_actn_lz { get; set; }
        public string csc_sync_flg_lz { get; set; }
        public string error_text_ls { get; set; }
        public byte[] audit_sbr_lz { get; set; }
    }

    public class CSC_PER_DATA_LZ_Response
    {
        public List<CSC_PER_DATA_LZ>? Result { get; set; }
        public int Status { get; set; }
    }
}
