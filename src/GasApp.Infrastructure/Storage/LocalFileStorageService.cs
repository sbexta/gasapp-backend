using GasApp.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace GasApp.Infrastructure.Storage;

public class LocalFileStorageService(IConfiguration configuration) : IFileStorageService
{
    private readonly string _basePath = configuration["Storage:BasePath"] ?? Path.Combine(AppContext.BaseDirectory, "uploads");

    public async Task<string> SaveAsync(Guid inspectionId, string fileName, byte[] data, CancellationToken ct = default)
    {
        var dir = Path.Combine(_basePath, inspectionId.ToString());
        Directory.CreateDirectory(dir);

        var uniqueName = $"{Guid.NewGuid()}_{fileName}";
        var fullPath = Path.Combine(dir, uniqueName);

        await File.WriteAllBytesAsync(fullPath, data, ct);

        return Path.Combine(inspectionId.ToString(), uniqueName);
    }

    public async Task<byte[]> ReadAsync(string storagePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath);
        return await File.ReadAllBytesAsync(fullPath, ct);
    }

    public Task DeleteAsync(string storagePath, CancellationToken ct = default)
    {
        var fullPath = Path.Combine(_basePath, storagePath);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
