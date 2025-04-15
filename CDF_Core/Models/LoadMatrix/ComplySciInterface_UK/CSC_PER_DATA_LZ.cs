
namespace CDF_Core.Models.LoadMatrix.ComplySciInterface_UK
{
    public class CSC_PER_DATA_LZ
    {
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
}
