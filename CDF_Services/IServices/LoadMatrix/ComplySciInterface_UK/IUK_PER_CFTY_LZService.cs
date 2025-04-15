using CDF_Core.Entities.LoadMatrix.ComplySciInterface_UK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK
{
    public interface IUK_PER_CFTY_LZService
    {
        public UK_PER_CFTY_LZ_Response getUK_PER_CFTY_LZData();
    }
}
