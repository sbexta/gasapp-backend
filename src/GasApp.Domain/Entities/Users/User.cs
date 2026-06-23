using GasApp.Domain.Enums;
using GasApp.Domain.Events;
using GasApp.Domain.Exceptions;
using GasApp.Domain.ValueObjects;

namespace GasApp.Domain.Entities.Users;

public class User : AuditableEntity
{
    public Email Email { get; private set; } = null!;
    public string HashedPassword { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? Phone { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsActive { get; private set; } = true;

    public string FullName => $"{FirstName} {LastName}";

    private User() { }

    public static User Create(Email email, string hashedPassword, string firstName, string lastName, UserRole role, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("El nombre no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("El apellido no puede estar vacío.");

        var user = new User
        {
            Email = email,
            HashedPassword = hashedPassword,
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Role = role,
            Phone = phone?.Trim()
        };

        user.AddDomainEvent(new UserCreatedEvent(user.Id, user.Email.Value, user.Role));
        return user;
    }

    public void UpdateProfile(string firstName, string lastName, string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new DomainException("El nombre no puede estar vacío.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new DomainException("El apellido no puede estar vacío.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Phone = phone?.Trim();
        Touch();
    }

    public void ChangePassword(string newHashedPassword)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword))
            throw new DomainException("La contraseña no puede estar vacía.");

        HashedPassword = newHashedPassword;
        Touch();
    }

    public void Activate()
    {
        IsActive = true;
        Touch();
    }

    public void Deactivate()
    {
        IsActive = false;
        Touch();
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        Touch();
    }
}
