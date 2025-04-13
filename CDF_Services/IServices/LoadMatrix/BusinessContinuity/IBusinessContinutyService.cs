using CDF_Core.Entities.LoadMatrix.BusinessContinuity;
using CDF_Core.Entities.PNP_Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.LoadMatrix.BusinessContinuity
{
    public interface IBusinessContinutyService
    {
        public CrisisMgmtResponse GetCrisisManagementData();

    }
}
