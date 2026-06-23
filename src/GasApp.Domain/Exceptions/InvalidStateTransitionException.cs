namespace GasApp.Domain.Exceptions;

public class InvalidStateTransitionException : DomainException
{
    public InvalidStateTransitionException(string from, string to)
        : base($"Transición inválida de '{from}' a '{to}'.") { }
}
