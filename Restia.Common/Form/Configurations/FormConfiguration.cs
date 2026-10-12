using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FormEntity = Restia.Common.Form.Entities.Form;

namespace Restia.Common.Form.Configurations
{
    public class FormConfiguration : IEntityTypeConfiguration<FormEntity>
    {
        public void Configure(EntityTypeBuilder<FormEntity> builder)
        {
            builder.HasKey(x => x.Id).HasName("pk_form");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Url)
                .HasMaxLength(500);

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
