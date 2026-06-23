using GasApp.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.ContractId).HasColumnName("contract_id").IsRequired();
        builder.Property(l => l.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(l => l.Address).HasColumnName("address").HasMaxLength(300).IsRequired();
        builder.Property(l => l.Municipality).HasColumnName("municipality").HasMaxLength(100).IsRequired();
        builder.Property(l => l.Department).HasColumnName("department").HasMaxLength(100).IsRequired();
        builder.Property(l => l.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(l => l.CreatedAt).HasColumnName("created_at");
        builder.Property(l => l.UpdatedAt).HasColumnName("updated_at");
        builder.Property(l => l.DeletedAt).HasColumnName("deleted_at");

        builder.OwnsOne(l => l.Coordinates, geo =>
        {
            geo.Property(g => g.Latitude).HasColumnName("latitude");
            geo.Property(g => g.Longitude).HasColumnName("longitude");
        });

        builder.HasIndex(l => l.ContractId).HasDatabaseName("ix_locations_contract_id");

        builder.HasOne(l => l.Contract)
            .WithMany()
            .HasForeignKey(l => l.ContractId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(l => l.DomainEvents);
        builder.Ignore(l => l.Installations);

        builder.HasQueryFilter(l => l.DeletedAt == null);
    }
}
