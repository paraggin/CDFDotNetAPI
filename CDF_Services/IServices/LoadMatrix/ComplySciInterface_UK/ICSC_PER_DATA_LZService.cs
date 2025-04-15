using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK
{
    public interface ICSC_PER_DATA_LZService
    {
        public CSC_PER_DATA_LZ_Response getCSC_PER_DATA_LZData();
    }
}
