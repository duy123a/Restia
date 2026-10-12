using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restia.Common.Permission.Entities;

namespace Restia.Common.Permission.Configurations
{
    internal class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.HasKey(e => new { e.RoleId, e.FormId }).HasName("pk_role_permission");
        }
    }
}
