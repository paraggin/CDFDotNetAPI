using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Messages
{
    public static class ErrorMessage
    {
        public static string Error_EmailOrPassword_Message = "Email or password is incorrect";
        public static int Error_EmailOrPassword_Code = 701;

        public static string Error_Passwordincorrect_Message = "password is incorrect";
        public static int Error_Passwordincorrect_Code = 702;

        public static string Error_EmailisAlready_Message = "Email is already registered!";
        public static int Error_EmailisAlready_Code = 703;

        public static string Error_Common_Message = "Error!";
        public static int Error_Common_Code = 704;

        public static string Error_AccountNotConfirmed_Message = "Account Not Confirmed!";
        public static int Error_AccountNotConfirmed_Code = 705;

        public static string Error_AccountNotConfirmedBefore_Message = "Account Confirmed Before!";
        public static int Error_AccountNotConfirmedBefore_Code = 706;

        public static string Error_AccountNotFound_Message = "User Not Found!";
        public static int Error_AccountNotFound_Code = 707;

        public static string Error_EmailisAlreadyExistAndActived_Message = "Email is already Subscribed!";
        public static int Error_EmailisAlreadyExistAndActived_Code = 708;

        public static string Error_EmailisNotExist_Message = "Email is Not Existed";
        public static int Error_EmailisNotExis_Code = 709;

        public static string Error_IdisNotExist_Message = "Id is Not Existed";
        public static int Error_IdisNotExist_Code = 710;

        public static string Error_FailedEmail_Message = "Failed To Send Email";
        public static int Error_FailedEmail_Code = 711;

        public static string Error_FailedRestPassword_Message = "Failed To Send Email";
        public static int Error_FailedRestPassword__Code = 712;
        public static string Error_AccountDeleted_Message = "Account Is Deleted by User!";
        public static int Error_AccountDeleted_Code = 713;

        public static string Error_EmailNotActive_Message = "This email is not active!";
        public static int Error_EmailNotActive_Code = 714;
    }
}
