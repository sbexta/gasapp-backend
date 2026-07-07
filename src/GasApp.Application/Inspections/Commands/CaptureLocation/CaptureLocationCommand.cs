using MediatR;

namespace GasApp.Application.Inspections.Commands.CaptureLocation;

public record CaptureLocationCommand(Guid InspectionId, double Latitude, double Longitude) : IRequest;
