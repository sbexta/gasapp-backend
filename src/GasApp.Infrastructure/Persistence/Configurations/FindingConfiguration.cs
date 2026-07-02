using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class FindingConfiguration : IEntityTypeConfiguration<Finding>
{
    public void Configure(EntityTypeBuilder<Finding> builder)
    {
        builder.ToTable("findings");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.InspectionId).HasColumnName("inspection_id").IsRequired();
        builder.Property(f => f.Description).HasColumnName("description").HasMaxLength(1000).IsRequired();
        builder.Property(f => f.Severity).HasColumnName("severity").HasMaxLength(50).IsRequired();
        builder.Property(f => f.RequiresCorrection).HasColumnName("requires_correction").IsRequired();
        builder.Property(f => f.CorrectiveAction).HasColumnName("corrective_action").HasMaxLength(1000);
        builder.Property(f => f.IsResolved).HasColumnName("is_resolved").IsRequired();
        builder.Property(f => f.ChecklistItemId).HasColumnName("checklist_item_id");
        builder.Property(f => f.CreatedAt).HasColumnName("created_at");
        builder.Property(f => f.UpdatedAt).HasColumnName("updated_at");
        builder.Property(f => f.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(f => f.InspectionId).HasDatabaseName("ix_findings_inspection_id");

        builder.HasQueryFilter(f => f.DeletedAt == null);
    }
}
