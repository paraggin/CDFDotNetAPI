/*using AutoMapper;
using CDF_Core.Entities.PNP_Accounts;
using CDF_Core.Interfaces;
using CDF_Infrastructure.Persistence.Data;
using CDF_Services.IServices.IPnpAccountServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDF_Services.Services.PnpAccountService
{
    public class PnpAccountService : IPnpAccountServices
    {
        private readonly IGenericRepository<PNP_Accounts> _IGenericRepository;
        private readonly IUnitOfWork<PNP_Accounts> _IUnitOfWork;
        private readonly ApplicationDBContext _dbContext;

        private readonly IMapper _mapper;
        public PnpAccountService(IGenericRepository<PNP_Accounts> iGenericRepository, IUnitOfWork<PNP_Accounts> iUnitOfWork, ApplicationDBContext dbContext, IMapper mapper)
        {
            _IGenericRepository = iGenericRepository;
            _IUnitOfWork = iUnitOfWork;
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public AccountSearchResult GetAccountByPolicy(string policy)
        {
            AccountSearchResult searchResult = new AccountSearchResult();

            if (policy != null)
            {
                searchResult.Result = _dbContext.PNP_Accounts.Where(p => p.policy == policy).ToList();
                searchResult.Status = 200; // Success status code
            }
            else
            {
                searchResult.Result = new List<PNP_Accounts>();
                searchResult.Status = 400; // Bad Request status code
            }

            return searchResult;
        }

    }
}

*/