using CDF_Core.Models.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Helper.Emails
{
    public interface IEmailGun
    {
        public HttpStatusCode sendEmail(string Email, string Fullname, string UserKey);
        public HttpStatusCode sendGmailEmail(EmailParameters emailParameters, Type type);

    }
}
