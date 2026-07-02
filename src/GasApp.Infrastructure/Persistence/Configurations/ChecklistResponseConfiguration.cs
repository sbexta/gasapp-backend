using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class ChecklistResponseConfiguration : IEntityTypeConfiguration<ChecklistResponse>
{
    public void Configure(EntityTypeBuilder<ChecklistResponse> builder)
    {
        builder.ToTable("checklist_responses");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.InspectionId).HasColumnName("inspection_id").IsRequired();
        builder.Property(r => r.ChecklistItemId).HasColumnName("checklist_item_id").IsRequired();
        builder.Property(r => r.TextValue).HasColumnName("text_value").HasMaxLength(2000);
        builder.Property(r => r.BoolValue).HasColumnName("bool_value");
        builder.Property(r => r.NumericValue).HasColumnName("numeric_value").HasPrecision(18, 4);
        builder.Property(r => r.Complies).HasColumnName("complies").IsRequired();
        builder.Property(r => r.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(r => new { r.InspectionId, r.ChecklistItemId })
            .IsUnique()
            .HasDatabaseName("ix_checklist_responses_inspection_item");

        builder.HasQueryFilter(r => r.DeletedAt == null);
    }
}
