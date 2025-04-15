using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;


namespace CDF_Services.Services.LoadMatric.ComplySciInterface_UK
{
    public class CSC_PER_DATA_LZService : ICSC_PER_DATA_LZService
    {
        private readonly ApplicationDBContext _dbContext;

        public CSC_PER_DATA_LZService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public CSC_PER_DATA_LZ_Response getCSC_PER_DATA_LZData()
        {

            CSC_PER_DATA_LZ_Response response = new CSC_PER_DATA_LZ_Response();

            response.Result = _dbContext.CSC_PER_DATA_LZ.ToList();
            response.Status = 200;

            return response;

        }
    }
}
