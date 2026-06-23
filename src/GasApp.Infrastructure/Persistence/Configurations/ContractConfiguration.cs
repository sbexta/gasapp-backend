using GasApp.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.ToTable("contracts");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.ClientId).HasColumnName("client_id").IsRequired();
        builder.Property(c => c.ContractNumber).HasColumnName("contract_number").HasMaxLength(50).IsRequired();
        builder.Property(c => c.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(c => c.EndDate).HasColumnName("end_date").IsRequired();
        builder.Property(c => c.Status).HasColumnName("status").HasMaxLength(50).IsRequired();
        builder.Property(c => c.Notes).HasColumnName("notes").HasMaxLength(1000);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(c => c.ContractNumber).IsUnique().HasDatabaseName("ix_contracts_number");
        builder.HasIndex(c => c.ClientId).HasDatabaseName("ix_contracts_client_id");

        builder.HasOne(c => c.Client)
            .WithMany()
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(c => c.DomainEvents);
        builder.Ignore(c => c.Locations);

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
