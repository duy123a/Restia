using Microsoft.EntityFrameworkCore;
using Restia.Common.Entities.Interfaces;
using Restia.Common.Shared.Entities;
using System.Linq.Expressions;

namespace Restia.Common.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ApplyCommonEntityConventions(
            this ModelBuilder builder)
        {
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var clrType = entityType.ClrType;

                // Soft Delete
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

                // Row Version
                if (typeof(BaseEntity).IsAssignableFrom(clrType))
                {
                    builder.Entity(clrType)
                        .Property(nameof(BaseEntity.RV))
                        .HasColumnName("xmin")
                        .HasColumnType("xid")
                        .IsRowVersion();
                }
            }

            return builder;
        }
    }
}