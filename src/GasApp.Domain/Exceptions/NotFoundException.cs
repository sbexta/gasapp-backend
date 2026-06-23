namespace GasApp.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entity, object id)
        : base($"{entity} con id '{id}' no fue encontrado.") { }
}
