using GasApp.Domain.Entities;
using GasApp.Domain.Exceptions;

namespace GasApp.Domain.Entities.Users;

public class UserSession : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string RefreshTokenHash { get; private set; } = null!;
    public string? DeviceInfo { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsValid => !IsExpired && !IsRevoked;

    private UserSession() { }

    public static UserSession Create(Guid userId, string refreshTokenHash, DateTime expiresAt, string? deviceInfo = null)
    {
        if (string.IsNullOrWhiteSpace(refreshTokenHash))
            throw new DomainException("El hash del refresh token no puede estar vacío.");

        return new UserSession
        {
            UserId = userId,
            RefreshTokenHash = refreshTokenHash,
            ExpiresAt = expiresAt,
            DeviceInfo = deviceInfo
        };
    }

    public void Revoke()
    {
        if (IsRevoked)
            throw new DomainException("La sesión ya fue revocada.");

        RevokedAt = DateTime.UtcNow;
        Touch();
    }
}
