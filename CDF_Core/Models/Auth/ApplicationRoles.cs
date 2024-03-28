using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Auth
{
    public class ApplicationRoles : IdentityRole
    {
        public override string Id { get; set; }
        [StringLength(250)]
        public string? RoleNameAr { get; set; }
        [StringLength(250)]
        public string? RoleNameEn { get; set; }
        [StringLength(250)]
        public string? RoleKey { get; set; }
        public bool? RolesIsActive { get; set; } = null;
    }
}
