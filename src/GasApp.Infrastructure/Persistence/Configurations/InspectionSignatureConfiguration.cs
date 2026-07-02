using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class InspectionSignatureConfiguration : IEntityTypeConfiguration<InspectionSignature>
{
    public void Configure(EntityTypeBuilder<InspectionSignature> builder)
    {
        builder.ToTable("inspection_signatures");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.InspectionId).HasColumnName("inspection_id").IsRequired();
        builder.Property(s => s.SignerName).HasColumnName("signer_name").HasMaxLength(200).IsRequired();
        builder.Property(s => s.SignerDocument).HasColumnName("signer_document").HasMaxLength(50);
        builder.Property(s => s.SignatureData).HasColumnName("signature_data").IsRequired();
        builder.Property(s => s.SignedAt).HasColumnName("signed_at").IsRequired();
        builder.Property(s => s.CreatedAt).HasColumnName("created_at");
        builder.Property(s => s.UpdatedAt).HasColumnName("updated_at");
        builder.Property(s => s.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(s => s.InspectionId).IsUnique().HasDatabaseName("ix_inspection_signatures_inspection_id");

        builder.HasQueryFilter(s => s.DeletedAt == null);
    }
}
