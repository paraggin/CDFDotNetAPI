using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.PNP_Accounts
{
    public class PNP_AccountsModel
    {
        public string? accountNumberSAP { get; set; }

        public int? account_name_SAP { get; set; }

        public string? account_definition { get; set; }
        public string? policy { get; set; }
    }

    public class AccountSearchResultModel
    {
        public List<PNP_AccountsModel> Result { get; set; }
        public int Status { get; set; }
    }
}
