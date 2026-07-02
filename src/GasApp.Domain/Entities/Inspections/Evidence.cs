using GasApp.Domain.Enums;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Inspections;

public class Evidence : AuditableEntity
{
    public Guid InspectionId { get; private set; }
    public EvidenceType Type { get; private set; }
    public string FileName { get; private set; } = null!;
    public string ContentType { get; private set; } = null!;
    public long FileSizeBytes { get; private set; }
    public string StoragePath { get; private set; } = null!;
    public Guid? ChecklistItemId { get; private set; }
    public string? Notes { get; private set; }
    public Guid UploadedBy { get; private set; }

    private Evidence() { }

    public static Evidence Create(Guid inspectionId, EvidenceType type, string fileName,
        string contentType, long fileSizeBytes, string storagePath, Guid uploadedBy,
        Guid? checklistItemId = null, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new DomainException("El nombre del archivo no puede estar vacío.");

        if (fileSizeBytes <= 0)
            throw new DomainException("El tamaño del archivo debe ser mayor a cero.");

        const long maxBytes = 10 * 1024 * 1024; // 10 MB
        if (fileSizeBytes > maxBytes)
            throw new DomainException("El archivo no puede superar 10 MB.");

        return new Evidence
        {
            InspectionId = inspectionId,
            Type = type,
            FileName = fileName.Trim(),
            ContentType = contentType.Trim(),
            FileSizeBytes = fileSizeBytes,
            StoragePath = storagePath.Trim(),
            UploadedBy = uploadedBy,
            ChecklistItemId = checklistItemId,
            Notes = notes?.Trim()
        };
    }
}
