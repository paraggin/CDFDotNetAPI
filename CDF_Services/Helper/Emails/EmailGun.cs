using RestSharp.Authenticators;
using RestSharp;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Data;
using System.Net.Mime;
using CDF_Services.Helper.Emails.Email_Templates;
using CDF_Core.Models.Email;
using Microsoft.AspNetCore.Hosting;

namespace CDF_Services.Helper.Emails
{
    public class EmailGun : IEmailGun
    {
        private readonly IConfiguration _configuration;
        //private readonly IWebHostEnvironment _env;

        public EmailGun(IConfiguration configuration)
        {
            //_env = env;
            _configuration = configuration;
        }
        public HttpStatusCode sendEmail(string Email, string Fullname, string UserKey)
        {
            RestClient client;
            string baseUrl = _configuration["EmailGunSetting:BaseUrl"];
            HttpClient http = new HttpClient();
            http.BaseAddress = new Uri(baseUrl);
            client = new RestClient(http, new RestClientOptions { BaseUrl = new Uri(baseUrl), Authenticator = new HttpBasicAuthenticator("api", _configuration["EmailGunSetting:APIKey"]) });

            RestRequest request = new RestRequest();
            request.AddParameter("domain", _configuration["EmailGunSetting:Domain"], ParameterType.UrlSegment);
            request.Resource = _configuration["EmailGunSetting:Resource"];
            request.AddParameter("from", _configuration["EmailGunSetting:SenderAddress"]);
            request.AddParameter("to", Email);
            request.AddParameter("subject", _configuration["EmailGunSetting:EmailSubject"]);
            //request.AddParameter("text", "Testing some Mailgun awesomness!");
            request.AddParameter("html", ConfirmEmailTemplate.Mail_BodyConfirmEmailTemplate(Fullname, _configuration["EmailGunSetting:EmailImage"], _configuration["EmailGunSetting:ReturnPath"] + UserKey));
            //request.AddFile("inline", "/logo.jpg");
            request.Method = Method.Post;
            //client.Execute(request);
            var response = client.Execute(request);
            return response.StatusCode;
            // dynamic content = Json.Decode(response.Content);
        }
        public static void WriteToFile22(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Log";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\EmailServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }

        }

        public HttpStatusCode sendGmailEmail(EmailParameters emailParameters, Type type)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.To.Add(emailParameters.Email);// Email-ID of Receiver  
                message.From = new System.Net.Mail.MailAddress(_configuration["EmailGunSetting:SenderAddress"], _configuration["EmailGunSetting:ProjectName"]);// Email-ID of Sender  
                message.IsBodyHtml = true;
                //var BasePath = Path.Combine(_env.WebRootPath, "EmailImages");
                var BasePath = Path.Combine("EmailImages");

                var logo = new Attachment(Path.Combine(BasePath, "ezstaff-logo.png"));
                logo.ContentId = "logo";
                logo.ContentDisposition.Inline = true;
                logo.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                var facebook = new Attachment(Path.Combine(BasePath, "facebook.png"));
                facebook.ContentId = "facebook";
                facebook.ContentDisposition.Inline = true;
                facebook.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                var linkedin = new Attachment(Path.Combine(BasePath, "linkedin.png"));
                linkedin.ContentId = "linkedin";
                linkedin.ContentDisposition.Inline = true;
                linkedin.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                var twitter = new Attachment(Path.Combine(BasePath, "twitter.png"));
                twitter.ContentId = "twitter";
                twitter.ContentDisposition.Inline = true;
                twitter.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                var pinterest = new Attachment(Path.Combine(BasePath, "pinterest.png"));
                pinterest.ContentId = "pinterest";
                pinterest.ContentDisposition.Inline = true;
                pinterest.ContentDisposition.DispositionType = DispositionTypeNames.Inline;

                message.Attachments.Add(logo);
                message.Attachments.Add(facebook);
                message.Attachments.Add(linkedin);
                message.Attachments.Add(twitter);
                message.Attachments.Add(pinterest);
                if (type == typeof(TemplateConfirmEmail))
                {
                    var ConfirmImage = new Attachment(Path.Combine(BasePath, "confirm-icon.png"));
                    ConfirmImage.ContentId = "ConfirmImage";
                    ConfirmImage.ContentDisposition.Inline = true;
                    ConfirmImage.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                    message.Attachments.Add(ConfirmImage);
                    message.Subject = _configuration["EmailGunSetting:EmailSubject"];
                    message.Body = TemplateConfirmEmail.GetTemplate(_configuration["EmailGunSetting:ReturnPath"] + emailParameters.UserKey);

                }
                else if (type == typeof(TemplateNewPasswordEmail))
                {
                    var PassImage = new Attachment(Path.Combine(BasePath, "pass-Icon.png"));
                    PassImage.ContentId = "PassImage";
                    PassImage.ContentDisposition.Inline = true;
                    PassImage.ContentDisposition.DispositionType = DispositionTypeNames.Inline;
                    message.Attachments.Add(PassImage);
                    message.Subject = _configuration["EmailGunSetting:NewPasswordSubject"];
                    message.Body = TemplateNewPasswordEmail.GetTemplate(emailParameters.Password);

                }
                else if (type == typeof(TemplateNewPasswordEmailReset))
                {
                    message.Subject = _configuration["EmailGunSetting:NewPasswordResetSubject"];
                    message.Body = TemplateNewPasswordEmailReset.GetTemplate(emailParameters.Password);
                }
                else if (type == typeof(TemplateTextMessage))
                {
                    message.Subject = _configuration["EmailGunSetting:TextMessageSubject"];
                    message.Body = TemplateTextMessage.GetTemplate(emailParameters.MessageSubject, emailParameters.Message);
                }
                else
                    return HttpStatusCode.NotFound;

                System.Net.Mail.SmtpClient SmtpMail = new System.Net.Mail.SmtpClient();
                SmtpMail.Host = "smtp.gmail.com";//name or IP-Address of Host used for SMTP transactions  
                SmtpMail.Port = 587;//Port for sending the mail 
                SmtpMail.Timeout = 60000;
                SmtpMail.UseDefaultCredentials = false;

                SmtpMail.Credentials = new System.Net.NetworkCredential(_configuration["EmailGunSetting:SenderAddress"], "llnvoeyehbzqkwrz");//username/password of network, if apply  
                SmtpMail.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpMail.EnableSsl = true;
                SmtpMail.ServicePoint.MaxIdleTime = 0;
                SmtpMail.ServicePoint.SetTcpKeepAlive(true, 2000, 2000);
                message.BodyEncoding = Encoding.Default;
                message.Priority = System.Net.Mail.MailPriority.High;

                try
                {
                    SmtpMail.Send(message); //Smtpclient to send the mail message  
                    return HttpStatusCode.Accepted;
                }
                catch (Exception ex)
                {
                    WriteToFile22("email error " + ex.Message);

                    throw ex;
                }

            }
            catch (Exception ex)
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:21308/InsuranceProject.aspx");
                HttpResponseMessage response = client.GetAsync("GetAllInsurance").Result;
                //WriteToFile22("email error " + ex.Message);
                throw ex;
            }

        }
    }
}
