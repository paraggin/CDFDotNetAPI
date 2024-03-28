using AutoMapper;
using CDF_Core.Models.Auth;
using CDF_Core.Models.Auth.Login;
using CDF_Core.Models.Auth.Register;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.Helper.Emails;
using CDF_Services.JwtSetting;
using CDF_Services.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Data;
using System.IdentityModel.Tokens.Jwt;

namespace CDF_Services.Auth
{
    public class AuthService : IAuthService
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly IMapper _mapper;
        private readonly JWT _jwt;
        private readonly ICreateToken _ICreateToken;
        private readonly ApplicationDBContext _dbContext;
        private readonly IEmailGun _emailGun;
        private readonly RoleManager<ApplicationRoles> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, IOptions<JWT> jwt, ICreateToken ICreateToken, ApplicationDBContext dbContext,
            IEmailGun emailGun, RoleManager<ApplicationRoles> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwt = jwt.Value;
            _ICreateToken = ICreateToken;
            _dbContext = dbContext;
            _emailGun = emailGun;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<AuthModel> LoginAsync(LoginModel model)
        {

            var user = _userManager.Users.SingleOrDefault(p => p.Email == model.Email);
            //// validate
            if (user is null)
                return new AuthModel { Message = ErrorMessage.Error_EmailOrPassword_Message, MessageCode = ErrorMessage.Error_EmailOrPassword_Code };

            //if (user is not null && user.EmailConfirmed != true)
            //    return new AuthModel { Message = ErrorMessage.Error_AccountNotConfirmed_Message, MessageCode = ErrorMessage.Error_AccountNotConfirmed_Code };

            if (user is not null && user.IsDeleted)
                return new AuthModel { Message = ErrorMessage.Error_AccountDeleted_Message, MessageCode = ErrorMessage.Error_AccountDeleted_Code };

            if (user is not null && user.EmailIsActive != true)
                return new AuthModel { Message = ErrorMessage.Error_EmailNotActive_Message, MessageCode = ErrorMessage.Error_EmailNotActive_Code };

            var result = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

            //// validate
            if (result != PasswordVerificationResult.Success)
                return new AuthModel { Message = ErrorMessage.Error_Passwordincorrect_Message, MessageCode = 704 };

            // authentication successful
            var userMap = _mapper.Map<ApplicationUser>(user);
            var jwtSecurityToken = await _ICreateToken.CreateJwtToken(userMap);

            //var Roles = _dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(d => d.RoleId).ToList();
            //var RoleName = _dbContext.Roles.Where(x => x. == user.Id).Select(d => d.).ToList();
            //var Roles = _dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(d => d.RoleId).ToList();
            //var RoleObj = _dbContext.Roles.Where(r => Roles.Contains(r.Id)).FirstOrDefault();
            //var Rolename = _dbContext.Roles.Where(r => Roles.Contains(r.Id)).Select(s => s.Name).ToList();
            //var RolenameAr = _dbContext.Roles.Where(r => Roles.Contains(r.Id)).Select(s => s.RoleNameAr).ToList();
            //var RolenameEn = _dbContext.Roles.Where(r => Roles.Contains(r.Id)).Select(s => s.RoleNameEn).ToList();
            return new AuthModel
            {
                Email = userMap.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                IsAuthentecated = true,
                //Roles = Roles,
                token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = userMap.UserName,
                IsCompletedInfo = userMap.IsCompletedInfo,
                Message = "",
                MessageCode = 200,
                //RoleNameAr = RoleObj.RoleNameAr,
                //RoleNameEn = RoleObj.RoleNameEn,
                //RolesName = Rolename,
                //RoleAr = RolenameAr,
                //RoleEn = RolenameEn
            };
            throw new NotImplementedException();
        }

        //<Guid("E5F0E8D1-BA1D-4D39-AF84-35ED47F6E6C7")>
        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
                return new AuthModel { Message = ErrorMessage.Error_EmailisAlready_Message, MessageCode = ErrorMessage.Error_EmailisAlready_Code };

            //if (await _userManager.FindByNameAsync(model.UserName) is not null)
            //    return new AuthModel { Message = "UserName is already registered!" };

            var user = _mapper.Map<ApplicationUser>(model);
            user.UserName = user.Email;
            user.CreationDate = DateTime.Now;
            user.CreationUserId = model.CreationUserId;
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},";
                }
                return new AuthModel { Message = errors, MessageCode = 704 };
            }

            //if (model.RolesList.Count > 0)
            //{

            //    //foreach (var role in model.RolesList)
            //    //{
            //    //    //if (!_dbContext.UserRoles.Any())
            //    //    //{
            //    //    var userRole = new IdentityUserRole<string>
            //    //    {
            //    //        UserId = user.Id,
            //    //        RoleId = role.Id.ToString()
            //    //    };
            //    //    _dbContext.UserRoles.Add(userRole);

            //    //    //}
            //    //}


            //}
            _dbContext.SaveChanges();

            //result = await _userManager.AddToRoleAsync(user, "USER");
            //if (!result.Succeeded)
            //    return new AuthModel { Message = "error!" };
            //var EmailParams = new EmailParameters();
            //EmailParams.Email = user.Email;
            //EmailParams.UserKey = user.Id;
            //var returnValue = _emailGun.sendGmailEmail(EmailParams, typeof(TemplateConfirmEmail));
            // var jwtSecurityToken = await _ICreateToken.CreateJwtToken(user);

            return new AuthModel
            {
                //Message = returnValue.ToString(),
                Email = user.Email,
                IsAuthentecated = true,
                Roles = _dbContext.UserRoles.Where(x => x.UserId == user.Id).Select(d => d.RoleId).ToList(),
                token = "",//new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = user.UserName,
                MessageCode = 200
            };
        }

        public async Task<AuthModel> ConfirmUser(string UID)
        {
            var user = _userManager.Users.SingleOrDefault(p => p.Id == UID);
            if (user is null)
                return new AuthModel { Message = ErrorMessage.Error_AccountNotFound_Message, MessageCode = ErrorMessage.Error_AccountNotFound_Code };

            if (user is not null && user.EmailConfirmed == true)
                return new AuthModel { Message = ErrorMessage.Error_AccountNotConfirmedBefore_Message, MessageCode = ErrorMessage.Error_AccountNotConfirmedBefore_Code };


            // authentication successful
            var userMap = _mapper.Map<ApplicationUser>(user);
            var jwtSecurityToken = await _ICreateToken.CreateJwtToken(userMap);
            if (userMap is not null)
            {
                userMap.Id = UID;
                userMap.EmailConfirmed = true;
                await _userManager.UpdateAsync(userMap);
            }
            return new AuthModel
            {
                Email = userMap.Email,
                ExpireOn = jwtSecurityToken.ValidTo,
                IsAuthentecated = true,
                Roles = _dbContext.UserRoles.Where(x => x.UserId == UID).Select(d => d.RoleId).ToList(),
                token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                UserName = userMap.UserName,
                Message = "",
                MessageCode = 200
            };
        }

        public async Task<bool> updateCompletedInfoFlagInUsers()
        {
            string userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
            var UserToEdit = await _userManager.FindByIdAsync(userId);
            UserToEdit.IsCompletedInfo = true;
            var res = await _userManager.UpdateAsync(UserToEdit);
            return res.Succeeded;
        }

    }
}