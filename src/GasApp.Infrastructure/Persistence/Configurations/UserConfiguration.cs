using GasApp.Domain.Entities.Users;
using GasApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("id");

        builder.Property(u => u.HashedPassword).HasColumnName("hashed_password").IsRequired();
        builder.Property(u => u.FirstName).HasColumnName("first_name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.LastName).HasColumnName("last_name").HasMaxLength(100).IsRequired();
        builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(30);
        builder.Property(u => u.Role).HasColumnName("role").HasMaxLength(50).IsRequired();
        builder.Property(u => u.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(u => u.CreatedAt).HasColumnName("created_at");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at");
        builder.Property(u => u.DeletedAt).HasColumnName("deleted_at");

        // Value Object Email como owned
        builder.OwnsOne(u => u.Email, email =>
        {
            email.Property(e => e.Value)
                .HasColumnName("email")
                .HasMaxLength(255)
                .IsRequired();
            email.HasIndex(e => e.Value).IsUnique().HasDatabaseName("ix_users_email");
        });

        // Ignorar domain events (no se persisten en DB)
        builder.Ignore(u => u.DomainEvents);

        // Soft delete query filter
        builder.HasQueryFilter(u => u.DeletedAt == null);
    }
}
