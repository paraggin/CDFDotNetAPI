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
    public class Leadership_LZService : ILeadership_LZService
    {
        private readonly ApplicationDBContext _dbContext;

        public Leadership_LZService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Leadership_LZResponse GetLeadershipLZData()
        {

            Leadership_LZResponse response = new Leadership_LZResponse();

            response.Result = _dbContext.Leadership_LZ.ToList();
            response.Status = 200;

            return response;

        }
    }
}
