using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Users
{
    public class AppUsersUpdateModel
    {
        public string Id { get; set; }
        public int RoleId { get; set; }
        public string Email { get; set; }
        public bool EmailIsActive { get; set; }

    }
}
