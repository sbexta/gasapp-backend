using GasApp.Domain.Entities.Checklists;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class ChecklistTemplateConfiguration : IEntityTypeConfiguration<ChecklistTemplate>
{
    public void Configure(EntityTypeBuilder<ChecklistTemplate> builder)
    {
        builder.ToTable("checklist_templates");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).HasColumnName("description").HasMaxLength(500);
        builder.Property(t => t.InspectionTypeId).HasColumnName("inspection_type_id");
        builder.Property(t => t.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(t => t.Version).HasColumnName("version").HasDefaultValue(1);
        builder.Property(t => t.CreatedAt).HasColumnName("created_at");
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at");
        builder.Property(t => t.DeletedAt).HasColumnName("deleted_at");

        builder.HasMany(t => t.Sections)
            .WithOne(s => s.Template)
            .HasForeignKey(s => s.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(t => t.DomainEvents);

        builder.HasQueryFilter(t => t.DeletedAt == null);
    }
}

public class ChecklistSectionConfiguration : IEntityTypeConfiguration<ChecklistSection>
{
    public void Configure(EntityTypeBuilder<ChecklistSection> builder)
    {
        builder.ToTable("checklist_sections");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.TemplateId).HasColumnName("template_id").IsRequired();
        builder.Property(s => s.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(s => s.Order).HasColumnName("order").IsRequired();
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.DeletedAt).HasColumnName("deleted_at");

        builder.HasMany(s => s.Items)
            .WithOne(i => i.Section)
            .HasForeignKey(i => i.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.DomainEvents);
    }
}

public class ChecklistItemConfiguration : IEntityTypeConfiguration<ChecklistItem>
{
    public void Configure(EntityTypeBuilder<ChecklistItem> builder)
    {
        builder.ToTable("checklist_items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.SectionId).HasColumnName("section_id").IsRequired();
        builder.Property(i => i.Question).HasColumnName("question").HasMaxLength(500).IsRequired();
        builder.Property(i => i.ItemType).HasColumnName("item_type").HasMaxLength(50).IsRequired();
        builder.Property(i => i.IsRequired).HasColumnName("is_required").HasDefaultValue(true);
        builder.Property(i => i.Order).HasColumnName("order").IsRequired();
        builder.Property(i => i.HelpText).HasColumnName("help_text").HasMaxLength(500);
        builder.Property(i => i.CreatedAt).HasColumnName("created_at");
        builder.Property(i => i.UpdatedAt).HasColumnName("updated_at");
        builder.Property(i => i.DeletedAt).HasColumnName("deleted_at");

        builder.Ignore(i => i.DomainEvents);
    }
}
