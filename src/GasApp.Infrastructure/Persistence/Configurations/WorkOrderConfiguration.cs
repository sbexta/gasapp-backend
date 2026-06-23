using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class WorkOrderConfiguration : IEntityTypeConfiguration<WorkOrder>
{
    public void Configure(EntityTypeBuilder<WorkOrder> builder)
    {
        builder.ToTable("work_orders");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.OrderNumber).HasColumnName("order_number").HasMaxLength(50).IsRequired();
        builder.Property(w => w.LocationId).HasColumnName("location_id").IsRequired();
        builder.Property(w => w.InspectionTypeId).HasColumnName("inspection_type_id").IsRequired();
        builder.Property(w => w.AssignedTechnicianId).HasColumnName("assigned_technician_id");
        builder.Property(w => w.ScheduledDate).HasColumnName("scheduled_date").IsRequired();
        builder.Property(w => w.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(w => w.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(w => w.CreatedAt).HasColumnName("created_at");
        builder.Property(w => w.UpdatedAt).HasColumnName("updated_at");
        builder.Property(w => w.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(w => w.OrderNumber).IsUnique().HasDatabaseName("ix_work_orders_number");
        builder.HasIndex(w => w.AssignedTechnicianId).HasDatabaseName("ix_work_orders_technician_id");
        builder.HasIndex(w => w.ScheduledDate).HasDatabaseName("ix_work_orders_scheduled_date");

        builder.HasOne(w => w.InspectionType)
            .WithMany()
            .HasForeignKey(w => w.InspectionTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(w => w.DomainEvents);

        builder.HasQueryFilter(w => w.DeletedAt == null);
    }
}
