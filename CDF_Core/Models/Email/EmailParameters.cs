using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Email
{
    public class EmailParameters
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ReturnPath { get; set; }
        public string MessageSubject { get; set; }
        public string Message { get; set; }
        public string UserKey { get; set; }
    }
}
