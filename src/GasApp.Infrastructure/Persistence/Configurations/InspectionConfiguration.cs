using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class InspectionConfiguration : IEntityTypeConfiguration<Inspection>
{
    public void Configure(EntityTypeBuilder<Inspection> builder)
    {
        builder.ToTable("inspections");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.WorkOrderId).HasColumnName("work_order_id").IsRequired();
        builder.Property(i => i.TechnicianId).HasColumnName("technician_id").IsRequired();
        builder.Property(i => i.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(i => i.StartedAt).HasColumnName("started_at");
        builder.Property(i => i.CompletedAt).HasColumnName("completed_at");
        builder.Property(i => i.TechnicianNotes).HasColumnName("technician_notes").HasMaxLength(2000);
        builder.Property(i => i.SupervisorNotes).HasColumnName("supervisor_notes").HasMaxLength(2000);
        builder.Property(i => i.CreatedAt).HasColumnName("created_at");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
        builder.Property(i => i.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(i => i.WorkOrderId).IsUnique().HasDatabaseName("ix_inspections_work_order_id");
        builder.HasIndex(i => i.TechnicianId).HasDatabaseName("ix_inspections_technician_id");

        builder.HasOne(i => i.WorkOrder)
            .WithMany()
            .HasForeignKey(i => i.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(i => i.DomainEvents);

        builder.HasQueryFilter(i => i.DeletedAt == null);
    }
}
