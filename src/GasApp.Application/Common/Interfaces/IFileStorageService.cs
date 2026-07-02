namespace GasApp.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Guid inspectionId, string fileName, byte[] data, CancellationToken ct = default);
    Task<byte[]> ReadAsync(string storagePath, CancellationToken ct = default);
    Task DeleteAsync(string storagePath, CancellationToken ct = default);
}
