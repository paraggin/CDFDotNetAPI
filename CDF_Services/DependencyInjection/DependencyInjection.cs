using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using CDF_Infrastructure.Persistence.Data;
using CDF_Infrastructure.UnitOfWork;
using CDF_Core.Interfaces;
using CDF_Services.JwtSetting;
using CDF_Infrastructure.Repository;
using CDF_Services.Services.PnpAccountService;
using CDF_Services.IServices.IPnpAccountServices;
using CDF_Services.Auth;
using CDF_Services.Helper.Emails;
using Microsoft.EntityFrameworkCore;
using Consfd = CDF_Services.Constants.Constants;
using CDF_Services.IServices.IBlobStorageService;
using CDF_Services.Services.BlobStorageService;
using CDF_Services.IServices.IHolidayCalendarServices;
using CDF_Services.Services.HolidayCalendarServices;


namespace CDF_Services.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection ImplementPersistence(this IServiceCollection services,
            IConfiguration configuration)
        {
            //services.AddDbContext<ApplicationDBContext>

            //services.AddDbContext<ApplicationDBContext>(option => option.UseSqlServer(
            // configuration.GetConnectionString("myConnection"),
            // b=>b.MigrationsAssembly(typeof(ApplicationDBContext).Assembly.FullName)),ServiceLifetime.Transient);

            services.AddDbContext<ApplicationDBContext>(option => option.UseSqlServer(
            configuration.GetConnectionString("myConnection")));
            services.AddSignalR();

            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ICreateToken, CreateToken>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<Consfd, Consfd>();

            #region Get PNP Account Data
            services.AddScoped<IPnpAccountServices, PnpAccountService>();
            #endregion

            services.AddScoped<IHolidayCalendarService, HolidayCalendarService>();
            services.AddScoped<IHolidayService, HolidayService>();
            services.AddScoped<IEvent_TypeService, Event_TypeService>();
            services.AddScoped<IWorkForceMasterService, WorkForceMasterService>();
            services.AddScoped<IITDepartmentService,DepartmentService>();

            #region Blob storage
            services.AddScoped<IBlobStorageService, BlobStorageService>();
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

            return services;
        }
    }
}
