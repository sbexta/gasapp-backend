using GasApp.Domain.Entities.Inspections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GasApp.Infrastructure.Persistence.Configurations;

public class InspectionCertificateConfiguration : IEntityTypeConfiguration<InspectionCertificate>
{
    public void Configure(EntityTypeBuilder<InspectionCertificate> builder)
    {
        builder.ToTable("inspection_certificates");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.InspectionId).HasColumnName("inspection_id").IsRequired();
        builder.Property(c => c.CertificateNumber).HasColumnName("certificate_number").HasMaxLength(50).IsRequired();
        builder.Property(c => c.IssuedAt).HasColumnName("issued_at").IsRequired();
        builder.Property(c => c.FilePath).HasColumnName("file_path").HasMaxLength(500).IsRequired(false);
        builder.Property(c => c.PdfData).HasColumnName("pdf_data").IsRequired();
        builder.Property(c => c.IssuedById).HasColumnName("issued_by_id").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");
        builder.Property(c => c.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(c => c.InspectionId).IsUnique().HasDatabaseName("ix_certificates_inspection_id");
        builder.HasIndex(c => c.CertificateNumber).IsUnique().HasDatabaseName("ix_certificates_number");

        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
