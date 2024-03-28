using CDF_Core.Models.Auth;
using CDF_Core.Models.Auth.Login;
using CDF_Core.Models.Auth.Register;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Auth
{
    public interface IAuthService
    {
        Task<AuthModel> LoginAsync(LoginModel Model);
        Task<AuthModel> RegisterAsync(RegisterModel Model);
        Task<AuthModel> ConfirmUser(string UID);
        Task<bool> updateCompletedInfoFlagInUsers();
    }
}