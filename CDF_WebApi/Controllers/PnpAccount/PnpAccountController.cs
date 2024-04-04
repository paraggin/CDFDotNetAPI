using CDF_Core.Entities.PNP_Accounts;
using CDF_Services.IServices.IPnpAccountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Constants = CDF_Services.Constants.Constants;

namespace CDF_WebApi.Controllers.PnpAccount
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class PnpAccountController : Controller
    {
        private readonly IPnpAccountServices _PnpAccountServices;
        private readonly Constants _IConstants;

        public PnpAccountController(IPnpAccountServices PnpAccountServices, Constants IConstants)
        {
            _PnpAccountServices = PnpAccountServices;
            _IConstants = IConstants;
        }   
        [HttpGet]
        [Route("getAccountByPolicy")]
        public AccountSearchResult GetAccountByPolicy(string policy)
        {
            return _PnpAccountServices.GetAccountByPolicy(policy);
        }
    }
}