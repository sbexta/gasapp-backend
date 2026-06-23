using GasApp.Domain.Exceptions;

namespace GasApp.Domain.ValueObjects;

public record GeoPoint
{
    public double Latitude { get; }
    public double Longitude { get; }

    public GeoPoint(double latitude, double longitude)
    {
        if (latitude < -90 || latitude > 90)
            throw new DomainException($"Latitud inválida: {latitude}. Debe estar entre -90 y 90.");

        if (longitude < -180 || longitude > 180)
            throw new DomainException($"Longitud inválida: {longitude}. Debe estar entre -180 y 180.");

        Latitude = latitude;
        Longitude = longitude;
    }

    public override string ToString() => $"{Latitude},{Longitude}";
}
