using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Core.Models.Users
{
    public class ProfileModel
    {
        public int? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public string? QID { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? NationalAddress { get; set; }
        public string? MailBox { get; set; }
        public string? PostalCode { get; set; }
        public string? FaxNumber { get; set; }
        public string? BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? NationalIdIssuedPlace { get; set; }
        public DateTime? NationalIdEndDate { get; set; }
        public string? RoleName { get; set; }
        public int? RoleId { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogOutDate { get; set; }
        public string? IPAddress { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? FkUserId { get; set; }
        public bool? UserInfoIsActive { get; set; }
        public int? FkUserCountryId { get; set; }
        public int? FkUserCityId { get; set; }
        public int? FkUserRegionId { get; set; }
        public int? FkUserNationalityId { get; set; }
        public int? FkUserRegisterTypeId { get; set; }
        public string? UserNotification { get; set; }
    }
}
