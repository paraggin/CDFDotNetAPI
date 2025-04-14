using CDF_Core.Entities.LoadMatrix.BusinessContinuity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.LoadMatrix.BusinessContinuity
{
    public interface ILeadership_LZService
    {
        public Leadership_LZResponse GetLeadershipLZData();
    }
}
