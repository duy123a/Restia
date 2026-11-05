using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Configurations
{
    public class NotificationConfiguration
    {
        public void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            builder.HasKey(x => x.Id).HasName("pk_notification");

            builder.Property(x => x.Url)
                .HasMaxLength(500);
        }
    }
}
