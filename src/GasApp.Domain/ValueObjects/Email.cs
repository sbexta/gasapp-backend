using GasApp.Domain.Exceptions;

namespace GasApp.Domain.ValueObjects;

public record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El correo electrónico no puede estar vacío.");

        var normalized = value.Trim().ToLowerInvariant();

        if (!normalized.Contains('@') || !normalized.Contains('.'))
            throw new DomainException($"El correo electrónico '{value}' no es válido.");

        Value = normalized;
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
