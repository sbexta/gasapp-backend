using GasApp.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class InstallationConfiguration : IEntityTypeConfiguration<Installation>
{
    public void Configure(EntityTypeBuilder<Installation> builder)
    {
        builder.ToTable("installations");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.LocationId).HasColumnName("location_id").IsRequired();
        builder.Property(i => i.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(i => i.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
        builder.Property(i => i.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(i => i.SerialNumber).HasColumnName("serial_number").HasMaxLength(100);
        builder.Property(i => i.Brand).HasColumnName("brand").HasMaxLength(100);
        builder.Property(i => i.Model).HasColumnName("model").HasMaxLength(100);
        builder.Property(i => i.InstallationYear).HasColumnName("installation_year");
        builder.Property(i => i.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(i => i.CreatedAt).HasColumnName("created_at");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
        builder.Property(i => i.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(i => i.LocationId).HasDatabaseName("ix_installations_location_id");

        builder.HasOne(i => i.Location)
            .WithMany()
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(i => i.DomainEvents);

        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
