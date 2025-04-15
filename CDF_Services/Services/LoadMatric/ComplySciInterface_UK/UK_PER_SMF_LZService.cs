using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Services.LoadMatric.ComplySciInterface_UK
{
    public class UK_PER_SMF_LZService : IUK_PER_SMF_LZService
    {
        private readonly ApplicationDBContext _dbContext;

        public UK_PER_SMF_LZService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }
        public UK_PER_SMF_LZ_Response getUK_PER_SMF_LZData()
        {
            UK_PER_SMF_LZ_Response response=new UK_PER_SMF_LZ_Response();
            response.Result= _dbContext.UK_PER_SMF_LZ.ToList();
            response.Status = 200;
            return response;
        }

    }
}
