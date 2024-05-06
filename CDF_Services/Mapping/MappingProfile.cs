using CDF_Core.Entities.Blob_Storage;
using CDF_Core.Entities.Holiday_Calendar;
using CDF_Core.Entities.PNP_Accounts;
using CDF_Core.Entities.Users;
using CDF_Core.Models.Auth;
using CDF_Core.Models.Auth.Register;
using CDF_Core.Models.Blob_Storage;
using CDF_Core.Models.Common;
using CDF_Core.Models.Holiday_Calender;
using CDF_Core.Models.PNP_Accounts;
using CDF_Core.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace CDF_Services.Mapping
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterModel, Profile>();
            CreateMap<RegisterModel, ApplicationUser>();
            //Lookup


            CreateMap<ProfileModel, Profile>();
            CreateMap<Profile, ProfileModel>();
            CreateMap<DeleteModel, Profile>();

            CreateMap<ApplicationRoles, IdentityRole>();

            CreateMap<AppUsersModel, ApplicationUser>();
            CreateMap<DeleteModel, ApplicationUser>();

            CreateMap<AppRolesModel, ApplicationRoles>();

            #region PnpAccount
            CreateMap<PNP_AccountsModel, PNP_Accounts>();
            CreateMap<AccountSearchResultModel, AccountSearchResult>();
            CreateMap<EventModel, Event>();
            CreateMap<HolidayModel, Holiday>();
            #endregion

            #region Blob Storage
            //CreateMap<Blob_StorageModel, Blob_Storage>();
            #endregion
        }

    }
}
