using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CDF_Infrastructure.Persistence.Data;
using CDF_Infrastructure.UnitOfWork;
using CDF_Core.Interfaces;
using CDF_Services.JwtSetting;
using CDF_Infrastructure.Repository;
using CDF_Services.Auth;
using CDF_Services.Helper.Emails;
using Microsoft.EntityFrameworkCore;
using Snowflake.Data.Client;
using Consfd = CDF_Services.Constants.Constants;
using CDF_Services.IServices.IBlobStorageService;
using CDF_Services.Services.BlobStorageService;
using CDF_Services.IServices.ISnowFlakeService;
using CDF_Services.Services.SnowFlakeService;
using CDF_Services.IServices.ILogViewerService;
using CDF_Services.Services.LogViewerService;
using CDF_Services.IServices.IDocViewerService;
using CDF_Services.Services.DocViewerService;
using CDF_Services.IServices.LoadMatrix.BusinessContinuity;
using CDF_Services.Services.LoadMatric.BusinessContinuty;
using CDF_Services.IServices.LoadMatrix.ComplySciInterface_UK;
using CDF_Services.Services.LoadMatric.ComplySciInterface_UK;

namespace CDF_Services.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection ImplementPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register EF Core DbContext
            services.AddDbContext<ApplicationDBContext>(option => option.UseSqlServer(
               configuration.GetConnectionString("hrConnection")));
            services.AddSignalR();

            // Register scoped services
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ICreateToken, CreateToken>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<Consfd, Consfd>();

            services.AddScoped<ICrisis_mgmtService,Crisis_mgmtService> ();
            services.AddScoped<ILeadership_LZService,Leadership_LZService>();
            services.AddScoped<ICSC_PER_DATA_LZService, CSC_PER_DATA_LZService>();
            services.AddScoped<IUK_PER_SMF_LZService,UK_PER_SMF_LZService>();
            services.AddScoped<IUK_PER_CFTY_LZService,UK_PER_CFTY_LZService>();
            services.AddScoped<ICSC_INST_GRP_LZService,CSC_INST_GRP_LZService>();

            #region Get PNP Account Data
         /*   services.AddScoped<IPnpAccountServices, PnpAccountService>();
            services.AddScoped<IHolidayCalendarService, HolidayCalendarService>();*/
            #endregion

            #region Blob storage
            services.AddScoped<IBlobStorageService, BlobStorageService>();
            services.AddScoped<IBlobKeyVaultService, BlobKeyVaultService>();
            services.AddScoped<ILogViewerService,LogViewerService>();
            services.AddScoped<IAzure_BlobstorageService, Azure_BlobstorageService>();
            services.AddScoped<IAzure_BlobstorageService2, Azure_BlobstorageService2>();
            services.AddScoped<IAzure_BlobstorageService3, Azure_BlobstorageService3>();

            services.AddScoped<IDocViewerService,DocViewerService>();


            #endregion

            #region Auth
            services.AddScoped<IAuthService, AuthService>();
            #endregion

           

            #region Email
            services.AddScoped<IEmailGun, EmailGun>();
            #endregion

            #region HttpContext
            services.AddHttpContextAccessor();
            #endregion

            #region Snowflake
            // Register SnowflakeDbConnection
            services.AddScoped<SnowflakeDbConnection>(provider =>
            {
                var connectionString = configuration.GetConnectionString("SnowflakeConnection");
                var connection = new SnowflakeDbConnection { ConnectionString = connectionString };
               /* connection.OpenAsync();
                // Test a simple query to verify connection
                using (var command = new SnowflakeDbCommand("SELECT CURRENT_VERSION()", connection))
                {
                    var version =  command.ExecuteScalarAsync();
                    Console.WriteLine($"Snowflake version: {version}");
                }*/
                return connection;
            });
            services.AddScoped<ISnowFlakeService,SnowFlakeService>();
            #endregion

            return services;
        }
    }
}
