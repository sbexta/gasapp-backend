using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Inspections;
using GasApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GasApp.Infrastructure.Certificates;

public class CertificateService(AppDbContext context) : ICertificateService
{
    public async Task<(string CertificateNumber, byte[] PdfBytes)> GenerateAsync(
        Guid inspectionId, CancellationToken ct = default)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var inspection = await context.Inspections
            .Include(i => i.WorkOrder).ThenInclude(w => w.InspectionType)
            .FirstOrDefaultAsync(i => i.Id == inspectionId, ct)
            ?? throw new InvalidOperationException($"Inspección {inspectionId} no encontrada.");

        var workOrder = inspection.WorkOrder;

        var location = await context.Locations
            .Include(l => l.Contract).ThenInclude(c => c.Client)
            .FirstOrDefaultAsync(l => l.Id == workOrder.LocationId, ct);

        var technician = await context.Users.FirstOrDefaultAsync(u => u.Id == inspection.TechnicianId, ct);

        var signature = await context.InspectionSignatures
            .FirstOrDefaultAsync(s => s.InspectionId == inspectionId, ct);

        var findings = await context.Findings
            .Where(f => f.InspectionId == inspectionId)
            .OrderByDescending(f => f.Severity)
            .ToListAsync(ct);

        var certNumber = $"CERT-{DateTime.UtcNow:yyyy}-{inspectionId.ToString("N")[..6].ToUpper()}";

        // Build typed data to avoid dynamic/extension method issue
        var data = new CertData(
            certNumber,
            workOrder.OrderNumber,
            workOrder.InspectionType?.Name ?? "—",
            workOrder.ScheduledDate,
            inspection.StartedAt,
            inspection.CompletedAt,
            technician?.FullName ?? "—",
            location?.Contract?.Client?.BusinessName ?? "—",
            location?.Name ?? "—",
            location?.Address ?? "—",
            location?.Municipality ?? "—",
            inspection.SupervisorNotes,
            findings,
            signature,
            inspection.LocationLat,
            inspection.LocationLng,
            inspection.LocationCapturedAt);

        var pdf = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Arial));
                page.Header().Element(c => ComposeHeader(c, data));
                page.Content().Element(c => ComposeContent(c, data));
                page.Footer().Element(ComposeFooter);
            });
        });

        var pdfBytes = pdf.GeneratePdf();
        return (certNumber, pdfBytes);
    }

    record CertData(
        string CertificateNumber, string OrderNumber, string InspectionTypeName,
        DateTime ScheduledDate, DateTime? StartedAt, DateTime? CompletedAt,
        string TechnicianName, string ClientName, string LocationName,
        string Address, string Municipality, string? SupervisorNotes,
        List<Finding> Findings, InspectionSignature? Signature,
        double? LocationLat, double? LocationLng, DateTime? LocationCapturedAt);

    static void ComposeHeader(IContainer container, CertData d)
    {
        container.Column(col =>
        {
            col.Item().BorderBottom(2).BorderColor("#1E40AF").PaddingBottom(8).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("GasApp — Sistema de Inspecciones").FontSize(14).Bold().FontColor("#1E40AF");
                    c.Item().Text("Organismo de Inspección de Gas Natural").FontSize(9).FontColor("#6B7280");
                });
                row.ConstantItem(140).AlignRight().Column(c =>
                {
                    c.Item().Text("CERTIFICADO DE INSPECCIÓN").FontSize(11).Bold().FontColor("#111827");
                    c.Item().Text(d.CertificateNumber).FontSize(9).FontColor("#374151");
                    c.Item().Text($"OT: {d.OrderNumber}").FontSize(9).FontColor("#374151");
                });
            });
            col.Item().Height(10);
        });
    }

    static void ComposeContent(IContainer container, CertData d)
    {
        container.Column(col =>
        {
            // Dates row
            col.Item().Background("#F3F4F6").Padding(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Fecha programada").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.ScheduledDate.ToString("dd/MM/yyyy")).Bold();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Fecha inicio").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.StartedAt.HasValue ? d.StartedAt.Value.ToString("dd/MM/yyyy HH:mm") : "—").Bold();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Fecha finalización").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.CompletedAt.HasValue ? d.CompletedAt.Value.ToString("dd/MM/yyyy HH:mm") : "—").Bold();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Técnico").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.TechnicianName).Bold();
                });
            });

            col.Item().Height(12);

            // Client & location
            col.Item().Text("INFORMACIÓN DEL CLIENTE Y SEDE").FontSize(9).Bold().FontColor("#1E40AF");
            col.Item().Height(4);
            col.Item().Border(1).BorderColor("#E5E7EB").Padding(10).Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Cliente").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.ClientName).Bold();
                    c.Item().Height(6);
                    c.Item().Text("Tipo de inspección").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.InspectionTypeName).Bold();
                });
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Sede").FontSize(8).FontColor("#6B7280");
                    c.Item().Text(d.LocationName).Bold();
                    c.Item().Height(6);
                    c.Item().Text("Dirección").FontSize(8).FontColor("#6B7280");
                    c.Item().Text($"{d.Address}, {d.Municipality}").Bold();
                });
            });

            col.Item().Height(12);

            // Findings
            col.Item().Text("HALLAZGOS").FontSize(9).Bold().FontColor("#1E40AF");
            col.Item().Height(4);

            if (d.Findings.Count == 0)
            {
                col.Item().Border(1).BorderColor("#E5E7EB").Padding(10)
                    .Text("Sin hallazgos registrados").FontColor("#6B7280").Italic();
            }
            else
            {
                col.Item().Border(1).BorderColor("#E5E7EB").Table(table =>
                {
                    table.ColumnsDefinition(tc =>
                    {
                        tc.RelativeColumn(3);
                        tc.RelativeColumn(1);
                        tc.RelativeColumn(1);
                    });
                    table.Header(h =>
                    {
                        h.Cell().Background("#F3F4F6").Padding(6).Text("Descripción").Bold().FontSize(9);
                        h.Cell().Background("#F3F4F6").Padding(6).Text("Severidad").Bold().FontSize(9);
                        h.Cell().Background("#F3F4F6").Padding(6).Text("Estado").Bold().FontSize(9);
                    });
                    foreach (var f in d.Findings)
                    {
                        table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6).Text(f.Description).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6).Text(f.Severity.ToString()).FontSize(9);
                        table.Cell().BorderBottom(1).BorderColor("#E5E7EB").Padding(6)
                            .Text(f.IsResolved ? "Resuelto" : "Pendiente").FontSize(9);
                    }
                });
            }

            if (d.LocationLat.HasValue && d.LocationLng.HasValue)
            {
                col.Item().Height(12);
                col.Item().Text("UBICACIÓN DE LA INSPECCIÓN").FontSize(9).Bold().FontColor("#1E40AF");
                col.Item().Height(4);
                col.Item().Border(1).BorderColor("#E5E7EB").Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Coordenadas GPS").FontSize(8).FontColor("#6B7280");
                        c.Item().Text($"{d.LocationLat:F6}°, {d.LocationLng:F6}°").Bold().FontSize(9);
                        if (d.LocationCapturedAt.HasValue)
                        {
                            c.Item().Height(4);
                            c.Item().Text("Capturada el").FontSize(8).FontColor("#6B7280");
                            c.Item().Text(d.LocationCapturedAt.Value.ToString("dd/MM/yyyy HH:mm") + " UTC").Bold().FontSize(9);
                        }
                    });
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Verificar en Google Maps").FontSize(8).FontColor("#6B7280");
                        c.Item().Text($"maps.google.com/?q={d.LocationLat:F6},{d.LocationLng:F6}").FontSize(8).FontColor("#1D4ED8");
                    });
                });
            }

            if (d.SupervisorNotes != null)
            {
                col.Item().Height(12);
                col.Item().Text("NOTAS DEL SUPERVISOR").FontSize(9).Bold().FontColor("#1E40AF");
                col.Item().Height(4);
                col.Item().Border(1).BorderColor("#E5E7EB").Padding(10).Text(d.SupervisorNotes).FontSize(9);
            }

            // Signature
            if (d.Signature != null)
            {
                col.Item().Height(16);
                col.Item().Text("CONFORMIDAD DEL CLIENTE").FontSize(9).Bold().FontColor("#1E40AF");
                col.Item().Height(4);
                col.Item().Border(1).BorderColor("#E5E7EB").Padding(10).Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        c.Item().Text("Firmante").FontSize(8).FontColor("#6B7280");
                        c.Item().Text(d.Signature.SignerName).Bold();
                        if (d.Signature.SignerDocument != null)
                        {
                            c.Item().Height(4);
                            c.Item().Text("Documento").FontSize(8).FontColor("#6B7280");
                            c.Item().Text(d.Signature.SignerDocument).Bold();
                        }
                        c.Item().Height(4);
                        c.Item().Text("Fecha firma").FontSize(8).FontColor("#6B7280");
                        c.Item().Text(d.Signature.SignedAt.ToString("dd/MM/yyyy HH:mm")).Bold();
                    });
                    row.ConstantItem(160).Column(c =>
                    {
                        c.Item().Text("Firma").FontSize(8).FontColor("#6B7280");
                        try
                        {
                            var sigData = d.Signature.SignatureData.Contains(",")
                                ? d.Signature.SignatureData.Split(",")[1]
                                : d.Signature.SignatureData;
                            c.Item().Height(60).Image(Convert.FromBase64String(sigData)).FitArea();
                        }
                        catch
                        {
                            c.Item().Height(60).Background("#F9FAFB")
                                .AlignCenter().AlignMiddle().Text("[firma]").FontColor("#9CA3AF");
                        }
                    });
                });
            }
        });
    }

    static void ComposeFooter(IContainer container)
    {
        container.BorderTop(1).BorderColor("#E5E7EB").PaddingTop(6).Row(row =>
        {
            row.RelativeItem().Text($"Generado el {DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC").FontSize(8).FontColor("#9CA3AF");
            row.RelativeItem().AlignRight().Text("GasApp — Sistema de Inspecciones de Gas").FontSize(8).FontColor("#9CA3AF");
        });
    }
}
