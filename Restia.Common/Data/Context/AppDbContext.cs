using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Extensions;
using Restia.Common.Identity.Entities;
using Restia.Common.Permission.Entities;
using System.Reflection;
using FormEntity = Restia.Common.Form.Entities.Form;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public DbSet<FormEntity> Forms => Set<FormEntity>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<UserPermission> UserPermissions => Set<UserPermission>();
        public DbSet<NotificationEntity> Notifications => Set<NotificationEntity>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.ApplyCommonEntityConventions();

        }
    }
}
