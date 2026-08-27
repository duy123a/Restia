using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restia.Common.Entities;
using Restia.Common.Entities.Interfaces;
using System.Linq.Expressions;

namespace Restia.Common.Data.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Wont see in the list by GET if it is SOFT DELETED
            ApplySoftDeleteFilters(builder);
        }

        private static void ApplySoftDeleteFilters(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                if (!typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    continue;
                }

                var parameter = Expression.Parameter(entityType.ClrType, "e");

                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));

                var body = Expression.Equal(property, Expression.Constant(false));

                var lambda = Expression.Lambda(body, parameter);

                builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
