using GasApp.Domain.Entities.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.BusinessName).HasColumnName("business_name").HasMaxLength(200).IsRequired();
        builder.Property(c => c.Nit).HasColumnName("nit").HasMaxLength(20).IsRequired();
        builder.Property(c => c.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
        builder.Property(c => c.ContactName).HasColumnName("contact_name").HasMaxLength(150);
        builder.Property(c => c.ContactPhone).HasColumnName("contact_phone").HasMaxLength(30);
        builder.Property(c => c.ContactEmail).HasColumnName("contact_email").HasMaxLength(255);
        builder.Property(c => c.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(c => c.Nit).IsUnique().HasDatabaseName("ix_clients_nit");

        builder.Ignore(c => c.DomainEvents);
        builder.Ignore(c => c.Contracts);

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
