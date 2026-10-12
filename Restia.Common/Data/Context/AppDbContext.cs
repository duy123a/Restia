using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Extensions;
using Restia.Common.Shared.Entities;
using System.Reflection;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public DbSet<FormEntity> Forms => Set<FormEntity>();

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
