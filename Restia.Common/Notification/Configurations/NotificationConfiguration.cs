using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<NotificationEntity>
    {
        public void Configure(EntityTypeBuilder<NotificationEntity> builder)
        {
            builder.HasKey(x => x.Id).HasName("pk_notification");

            builder.Property(x => x.TitleKey)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MessageKey)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Url)
                .HasMaxLength(500);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Sender)
                .WithMany()
                .HasForeignKey(x => x.SenderId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
