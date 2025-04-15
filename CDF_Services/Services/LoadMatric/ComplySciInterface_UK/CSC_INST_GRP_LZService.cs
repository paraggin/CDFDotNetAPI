using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;

namespace CDF_Services.Services.LoadMatric.ComplySciInterface_UK
{
    public class CSC_INST_GRP_LZService : ICSC_INST_GRP_LZService
    {          

        private readonly ApplicationDBContext _dbContext;

        public CSC_INST_GRP_LZService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public CSC_INST_GRP_LZResponse getCSC_INST_GRP_LZData()
        {

            CSC_INST_GRP_LZResponse response = new CSC_INST_GRP_LZResponse();

            response.Result = _dbContext.CSC_INST_GRP_LZ.ToList();
            response.Status = 200;

            return response;

        }
    }

}
