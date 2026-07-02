using GasApp.Domain.Entities;
using GasApp.Domain.Entities.Checklists;
using GasApp.Domain.Entities.Clients;
using GasApp.Domain.Entities.Inspections;
using GasApp.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace GasApp.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Installation> Installations => Set<Installation>();

    public DbSet<InspectionType> InspectionTypes => Set<InspectionType>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Inspection> Inspections => Set<Inspection>();

    public DbSet<ChecklistTemplate> ChecklistTemplates => Set<ChecklistTemplate>();
    public DbSet<ChecklistSection> ChecklistSections => Set<ChecklistSection>();
    public DbSet<ChecklistItem> ChecklistItems => Set<ChecklistItem>();

    public DbSet<ChecklistResponse> ChecklistResponses => Set<ChecklistResponse>();
    public DbSet<Evidence> Evidences => Set<Evidence>();
    public DbSet<Finding> Findings => Set<Finding>();
    public DbSet<InspectionSignature> InspectionSignatures => Set<InspectionSignature>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Guardar todos los enums como string en la base de datos
        configurationBuilder.Properties<Enum>().HaveConversion<string>();
    }
}
