using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Constants
{
    public interface IConstants
    {
        public string GetResponseOk();

        public string GetResponseOk(string message);

        public string GetResponseError(string error);
        public string GetResponseGenericSuccess(dynamic Res);
        public string getGeneratedCode(string code);
    }
}
