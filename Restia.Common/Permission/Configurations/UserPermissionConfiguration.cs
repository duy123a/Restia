using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restia.Common.Permission.Entities;

namespace Restia.Common.Permission.Configurations
{
    internal class UserPermissionConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.HasKey(e => new { e.UserId, e.FormCode }).HasName("pk_user_permission");
        }
    }
}
