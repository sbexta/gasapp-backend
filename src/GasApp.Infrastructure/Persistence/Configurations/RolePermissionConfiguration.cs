using GasApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");
        builder.Property(r => r.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
        builder.Property(r => r.Permission).HasColumnName("permission").HasMaxLength(100).IsRequired();
        builder.Property(r => r.IsGranted).HasColumnName("is_granted").HasDefaultValue(false);
        builder.Property(r => r.CreatedAt).HasColumnName("created_at");
        builder.Property(r => r.UpdatedAt).HasColumnName("updated_at");
        builder.Property(r => r.DeletedAt).HasColumnName("deleted_at");
        builder.Ignore(r => r.DomainEvents);
        builder.HasIndex(r => new { r.Role, r.Permission }).IsUnique().HasDatabaseName("ix_role_permissions_role_permission");
        builder.HasQueryFilter(r => r.DeletedAt == null);
    }
}
