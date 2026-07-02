namespace GasApp.Domain.Enums;

public enum AppPermission
{
    // Usuarios
    ManageUsers,
    ViewUsers,

    // Clientes y contratos
    ManageClients,
    ViewClients,
    ManageContracts,
    ManageLocations,

    // Tipos e inspecciones
    ManageInspectionTypes,
    ManageWorkOrders,
    AssignTechnicians,
    StartInspections,

    // Ejecución
    PerformChecklist,
    RegisterFindings,
    CaptureSignature,

    // Revisión
    ViewInspections,
    ApproveInspections,
}
