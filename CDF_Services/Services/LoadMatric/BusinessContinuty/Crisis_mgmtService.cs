using CDF_Core.Entities.LoadMatrix.BusinessContinuity;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.LoadMatrix.BusinessContinuity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Services.LoadMatric.BusinessContinuty
{
    public class Crisis_mgmtService : ICrisis_mgmtService
    {
        private readonly ApplicationDBContext _dbContext;

        public Crisis_mgmtService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public CrisisMgmtResponse GetCrisisManagementData()
        {
           
            CrisisMgmtResponse response=new CrisisMgmtResponse();

            response.Result=_dbContext.Crisis_Mgmt.ToList();
            response.Status = 200;

            return response;

        }
    }
}
