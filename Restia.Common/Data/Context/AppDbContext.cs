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

            ApplyEntityConfigurations(builder);

        }

        private static void ApplyEntityConfigurations(ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                if (typeof(ISoftDeletable).IsAssignableFrom(clrType))
                {
                    var parameter = Expression.Parameter(clrType, "e");

                    var property = Expression.Property(
                        parameter,
                        nameof(ISoftDeletable.IsDeleted));

                    var body = Expression.Equal(
                        property,
                        Expression.Constant(false));

                    var lambda = Expression.Lambda(body, parameter);

                    builder.Entity(clrType)
                        .HasQueryFilter(lambda);
                }

                if (typeof(BaseEntity).IsAssignableFrom(clrType))
                {
                    builder.Entity(clrType)
                        .Property(nameof(BaseEntity.RV))
                        .HasColumnName("xmin")
                        .HasColumnType("xid")
                        .IsRowVersion();
                }
            }
        }
    }
}
