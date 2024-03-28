using CDF_Core.Models.Auth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.JwtSetting
{
    public interface ICreateToken
    {
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user);
    }
}
