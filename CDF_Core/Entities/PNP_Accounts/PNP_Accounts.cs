using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.PNP_Accounts
{
    public class PNP_Accounts 
    {
        public PNP_Accounts() { }
        public long id { get; set; }

        public int? account_number_SAP { get; set; }

        public string account_name_SAP { get; set; }

        public string account_definition { get; set; }
        public string policy { get; set; }
    }

    public class AccountSearchResult
    {
        public List<PNP_Accounts>? Result { get; set; }
        public int Status { get; set; }
    }
}