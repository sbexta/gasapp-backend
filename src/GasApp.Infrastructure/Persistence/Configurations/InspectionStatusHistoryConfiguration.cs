using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class InspectionStatusHistoryConfiguration : IEntityTypeConfiguration<InspectionStatusHistory>
{
    public void Configure(EntityTypeBuilder<InspectionStatusHistory> builder)
    {
        builder.ToTable("inspection_status_history");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.InspectionId).HasColumnName("inspection_id").IsRequired();
        builder.Property(h => h.PreviousStatus).HasColumnName("previous_status").HasMaxLength(50);
        builder.Property(h => h.NewStatus).HasColumnName("new_status").HasMaxLength(50).IsRequired();
        builder.Property(h => h.ChangedAt).HasColumnName("changed_at").IsRequired();
        builder.Property(h => h.ChangedById).HasColumnName("changed_by_id");
        builder.Property(h => h.Notes).HasColumnName("notes").HasMaxLength(1000);

        builder.HasIndex(h => h.InspectionId).HasDatabaseName("ix_inspection_history_inspection_id");
    }
}
