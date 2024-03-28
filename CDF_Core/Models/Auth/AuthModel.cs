using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Auth
{
    public class AuthModel
    {
        public string Message { get; set; }
        public int? MessageCode { get; set; }
        public bool? IsCompletedInfo { get; set; }
        public bool IsAuthentecated { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public List<string> Roles { get; set; }
        public string? token { get; set; }
        public DateTime ExpireOn { get; set; }
        public string RoleNameAr { get; set; }
        public string RoleNameEn { get; set; }
        public List<string> RolesName { get; set; } = null;
        public string RoleId { get; set; }
        public List<string> RoleAr { get; set; } = null;
        public List<string> RoleEn { get; set; } = null;

    }
}
