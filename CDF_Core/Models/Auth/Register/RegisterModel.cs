using CDF_Core.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Auth.Register
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? CreationUserId { get; set; }
        public ICollection<AppRolesModel> RolesList { get; set; }

    }
}
