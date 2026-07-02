using GasApp.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("notifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(n => n.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
        builder.Property(n => n.Body).HasColumnName("body").HasMaxLength(1000).IsRequired();
        builder.Property(n => n.Type).HasColumnName("type").HasMaxLength(50).IsRequired();
        builder.Property(n => n.ReferenceId).HasColumnName("reference_id");
        builder.Property(n => n.IsRead).HasColumnName("is_read").IsRequired();
        builder.Property(n => n.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(n => n.UserId).HasDatabaseName("ix_notifications_user_id");
        builder.HasIndex(n => new { n.UserId, n.IsRead }).HasDatabaseName("ix_notifications_user_unread");
    }
}
