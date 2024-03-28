using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Users
{
    public class AppUsersModel
    {
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? CreationUserId { get; set; }
        public int? RoleId { get; set; }
        public ICollection<AppRolesModel>? RolesList { get; set; }
        public bool? EmailIsActive { get; set; }
        public bool? EmailConfirmed { get; set; }
        public string? Name { get; set; }

        public string? RoleIds { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? ProfileId { get; set; }

    }
}
