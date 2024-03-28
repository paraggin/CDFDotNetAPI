using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using CDF_Core.Entities;
using CDF_Core.Entities.Users;
using CDF_Core.Entities.PNP_Accounts;
using Microsoft.AspNetCore.Http;
using CDF_Core.Models.Auth;
using CDF_Core.Entities.Blob_Storage;

namespace CDF_Infrastructure.Persistence.Data
{
    public class ApplicationDBContext : IdentityDbContext<ApplicationUser, ApplicationRoles, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApplicationDBContext(IHttpContextAccessor httpContextAccessor, DbContextOptions<ApplicationDBContext> options) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<Profile> UserInfo { get; set; }
        public DbSet<RegisterType> RegisterType { get; set; }

        public DbSet<PNP_Accounts> PNP_Accounts { get; set; }
        //public DbSet<Blob_Storage> Blob_Storage { get; set; }

        public override int SaveChanges()
        {
            var changedEntities = ChangeTracker.Entries<TransactionEntityBase>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();
            if (changedEntities.Count > 0)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var CurrentUserInfoId = UserInfo.Where(u => u.FkUserId == userId).FirstOrDefault()?.Id;
                foreach (var entityEntry in changedEntities)
                {
                    var entityName = entityEntry.Entity.GetType().Name;
                    if (entityEntry.State == EntityState.Added)
                    {
                        if (entityName == "Events")
                        {
                            entityEntry.Entity.CreationDate = DateTime.UtcNow;
                        }
                        else
                        {
                            entityEntry.Entity.CreationDate = DateTime.Now;
                        }
                        entityEntry.Entity.CreationUserId = (int)CurrentUserInfoId;
                    }
                    else if (entityEntry.State == EntityState.Modified)
                    {
                        if (entityName == "Events")
                        {
                            entityEntry.Entity.LastUpdateDate = DateTime.UtcNow;
                        }
                        else
                        {
                            entityEntry.Entity.LastUpdateDate = DateTime.Now;
                        }
                        entityEntry.Entity.LastUpdateUserId = CurrentUserInfoId;
                    }
                }
            }

            return base.SaveChanges();
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var changedEntities = ChangeTracker.Entries<TransactionEntityBase>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .ToList();
            if (changedEntities.Count > 0)
            {
                var userId = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "uid")?.Value;
                var CurrentProfileId = UserInfo.Where(u => u.FkUserId == userId).FirstOrDefault()?.Id;
                foreach (var entityEntry in changedEntities)
                {
                    if (entityEntry.State == EntityState.Added)
                    {
                        entityEntry.Entity.CreationDate = DateTime.UtcNow;
                        entityEntry.Entity.CreationUserId = (int)CurrentProfileId;
                    }
                    else if (entityEntry.State == EntityState.Modified)
                    {
                        entityEntry.Entity.LastUpdateDate = DateTime.UtcNow;
                        entityEntry.Entity.LastUpdateUserId = CurrentProfileId;
                    }
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}