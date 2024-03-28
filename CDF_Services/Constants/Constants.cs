using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Constants
{
    public class Constants
    {

        public const string LanguageEN = "en-US";
        public const string LanguageAR = "ar-EG";

        public const string ContentTypeJson = "application/json";
        public const string ContentTypeText = "text/plain";
        public const string ALL_RECORDS = "-2";
        public const string TOKEN_HEADER_NAME = "x-ez-token";
        public Constants()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string GetResponseOk()
        {
            string s = "{\"result\":{\"result\":\"OK\"" + ",\"details\":\"" + "NO ERROR" + "\"}}";
            return s;

        }

        public string GetResponseOk(string message)
        {
            string s = "{\"result\":{\"result\":\"OK\"" + ",\"details\":\"" + message + "\"}}";
            return s;
        }

        public string GetResponseError(string error)
        {
            string s = "{\"result\":{\"result\":\"ERROR\"" + ",\"details\":\"" + error + "\"}}";
            return s;

        }

        public string GetResponseGenericSuccess(dynamic Res)
        {
            string s = "{\"result\":{\"result\":\"OK\",\"details\":\"NO ERROR\"},\"items\": ";
            s = s + JsonConvert.SerializeObject(Res, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            s = s + "}";
            return s;

        }

        public string GetResponseGenericSuccessFromSpWithPaging(string Res)
        {
            string s = "{\"items\":";
            s = s + Res;
            s = s + "}";
            return s;
        }

        public string GetResponseNotFound(string errorKey)
        {
            return errorKey + " is not found";
        }

        public static string getGeneratedCode(string TableKey)
        {
            string CodeId = TableKey + DateTime.Now.Ticks.ToString().Substring(0, 17);
            return CodeId;
        }
        public static string GeneratePassword(PasswordOptions options)
        {
            int length = options.RequiredLength;

            bool nonAlphanumeric = options.RequireNonAlphanumeric;
            bool digit = options.RequireDigit;
            bool lowercase = options.RequireLowercase;
            bool uppercase = options.RequireUppercase;

            StringBuilder password = new StringBuilder();
            Random random = new Random();

            while (password.Length < length)
            {
                char c = (char)random.Next(32, 126);

                password.Append(c);

                if (char.IsDigit(c))
                    digit = false;
                else if (char.IsLower(c))
                    lowercase = false;
                else if (char.IsUpper(c))
                    uppercase = false;
                else if (!char.IsLetterOrDigit(c))
                    nonAlphanumeric = false;
            }

            if (nonAlphanumeric)
                password.Append((char)random.Next(33, 48));
            if (digit)
                password.Append((char)random.Next(48, 58));
            if (lowercase)
                password.Append((char)random.Next(97, 123));
            if (uppercase)
                password.Append((char)random.Next(65, 91));

            return password.ToString() + DateTime.Now.Ticks.ToString().Substring(0, 10);
        }


        public static string throwExeption(string errorKey)
        {
            throw new Exception(errorKey + " is not found");
        }
        public string getResponseFailed(string errorKey)
        {
            return $"Failed to Reteive {errorKey}";
        }
        public string getResponseAlreadyExist(string errorKey)
        {
            return $"{errorKey} is already exists";
        }
        public int TimeZone = 3;
    }
}
