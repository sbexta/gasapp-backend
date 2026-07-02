using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Repositories;
using GasApp.Infrastructure.Auth;
using GasApp.Infrastructure.Persistence;
using GasApp.Infrastructure.Persistence.Repositories;
using GasApp.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GasApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserSessionRepository, UserSessionRepository>();

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IContractRepository, ContractRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IInstallationRepository, InstallationRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<IInspectionRepository, InspectionRepository>();
        services.AddScoped<IChecklistTemplateRepository, ChecklistTemplateRepository>();
        services.AddScoped<IChecklistSectionRepository, ChecklistSectionRepository>();
        services.AddScoped<IChecklistItemRepository, ChecklistItemRepository>();
        services.AddScoped<IChecklistResponseRepository, ChecklistResponseRepository>();
        services.AddScoped<IEvidenceRepository, EvidenceRepository>();
        services.AddScoped<IFindingRepository, FindingRepository>();
        services.AddScoped<IInspectionSignatureRepository, InspectionSignatureRepository>();
        services.AddScoped<IInspectionTypeRepository, InspectionTypeRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BcryptPasswordHasher>();

        return services;
    }
}
