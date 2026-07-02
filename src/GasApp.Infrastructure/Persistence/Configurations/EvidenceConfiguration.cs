using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class EvidenceConfiguration : IEntityTypeConfiguration<Evidence>
{
    public void Configure(EntityTypeBuilder<Evidence> builder)
    {
        builder.ToTable("evidences");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.InspectionId).HasColumnName("inspection_id").IsRequired();
        builder.Property(e => e.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
        builder.Property(e => e.FileName).HasColumnName("file_name").HasMaxLength(255).IsRequired();
        builder.Property(e => e.ContentType).HasColumnName("content_type").HasMaxLength(100).IsRequired();
        builder.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes").IsRequired();
        builder.Property(e => e.StoragePath).HasColumnName("storage_path").HasMaxLength(500).IsRequired();
        builder.Property(e => e.ChecklistItemId).HasColumnName("checklist_item_id");
        builder.Property(e => e.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(e => e.UploadedBy).HasColumnName("uploaded_by").IsRequired();
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(e => e.InspectionId).HasDatabaseName("ix_evidences_inspection_id");

        builder.HasQueryFilter(e => e.DeletedAt == null);
    }
}
