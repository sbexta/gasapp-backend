using MediatR;

namespace GasApp.Application.Inspections.Commands.CaptureSignature;

public record CaptureSignatureCommand(
    Guid InspectionId,
    string SignerName,
    string SignatureData,
    string? SignerDocument
) : IRequest;
