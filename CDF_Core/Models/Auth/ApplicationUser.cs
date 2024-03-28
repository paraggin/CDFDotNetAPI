using CDF_Core.Entities.Users;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;



namespace CDF_Core.Models.Auth
{
    public class ApplicationUser : IdentityUser
    {
        public bool? IsCompletedInfo { get; set; } = false;
        public string? CreationUserId { get; set; }
        public int? RoleId { get; set; }
        public DateTime? CreationDate { get; set; }

        [ForeignKey("FkUserId")]
        public ICollection<Profile> UsersInfo { get; set; } 
        public bool EmailIsActive { get; set; } = true;

        public bool IsDeleted { set; get; } = false;
    }
}
