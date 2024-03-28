using CDF_Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Entities.Users
{
    [Table("UserInfo")]
    public class Profile : EntityBase
    {
        public Profile() { }

        [StringLength(250)]
        public string FirstName { get; set; }
        [StringLength(250)]
        public string LastName { get; set; }
        [StringLength(250)]
        public string? FullName { get; set; }
        [StringLength(250)]
        public string? CompanyName { get; set; }
        [StringLength(250)]
        public string? CompanyAddress { get; set; }
        [StringLength(20)]
        public string? QID { get; set; }
        [StringLength(20)]
        public string? PhoneNumber { get; set; }
        [StringLength(20)]
        public string? CountryCode { get; set; }
        [StringLength(250)]
        public string? NationalAddress { get; set; }
        [StringLength(250)]
        public string? MailBox { get; set; }
        [StringLength(250)]
        public string? PostalCode { get; set; }
        [StringLength(250)]
        public string? FaxNumber { get; set; }
        [StringLength(250)]
        public string? BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        [StringLength(250)]
        public string? NationalIdIssuedPlace { get; set; }
        public DateTime? NationalIdEndDate { get; set; }
        [StringLength(250)]
        public string? RoleName { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastLoginDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LastLogOutDate { get; set; }
        [StringLength(50)]
        public string? IPAddress { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreationDate { get; set; }
        [StringLength(500)]
        public string? ImageUrl { get; set; }
        public string FkUserId { get; set; }
        public bool UserInfoIsActive { get; set; }
        public int? FkUserCountryId { get; set; }
        public int? FkUserCityId { get; set; }
        public int? FkUserRegionId { get; set; }
        public int? FkUserNationalityId { get; set; }
        public int? FkUserRegisterTypeId { get; set; }
        public RegisterType RegisterType { get; set; }

        [NotMapped]
        public virtual ApplicationUser ApplicationUser { get; set; }

        //[ForeignKey("FkUserAssignedId")]
        //public virtual ICollection<Tasks> Tasks { get; set; }
        [StringLength(50)]
        public string? UserNotification { get; set; }
    }
}
