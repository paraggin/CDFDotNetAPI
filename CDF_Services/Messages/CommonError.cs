using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Messages
{
    public class CommonError
    {

        public string message { get; set; }
        public int status { get; set; }

        public CommonError(string message, int status)
        {
            this.message = message;
            this.status = status;
        }
        public override string ToString()
        {
            return $"{{ \"statusCode\": {status}, \"Message\": \"{message}\" }}";
        }
    }
}