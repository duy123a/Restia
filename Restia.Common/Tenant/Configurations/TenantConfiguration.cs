using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TenantEntity = Restia.Common.Tenant.Entities.Tenant;

namespace Restia.Common.Tenant.Configurations
{
    public class TenantConfiguration : IEntityTypeConfiguration<TenantEntity>
    {
        public void Configure(EntityTypeBuilder<TenantEntity> builder)
        {
            builder.HasKey(x => x.Id).HasName("pk_tenant");

            builder.Property(x => x.Id)
                .IsRequired()
                .HasMaxLength(63);

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
