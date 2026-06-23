using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class InspectionTypeConfiguration : IEntityTypeConfiguration<InspectionType>
{
    public void Configure(EntityTypeBuilder<InspectionType> builder)
    {
        builder.ToTable("inspection_types");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(150).IsRequired();
        builder.Property(t => t.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(t => t.RequiresCertificate).HasColumnName("requires_certificate").HasDefaultValue(true);
        builder.Property(t => t.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        builder.Property(t => t.DeletedAt).HasColumnName("deleted_at");

        builder.Ignore(t => t.DomainEvents);

        builder.HasQueryFilter(t => t.DeletedAt == null);
    }
}
