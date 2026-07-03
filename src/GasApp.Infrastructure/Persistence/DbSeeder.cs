using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Enums;
using GasApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GasApp.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher, ILogger logger, IConfiguration configuration)
    {
        await context.Database.MigrateAsync();

        await SeedAdminUserAsync(context, passwordHasher, logger, configuration);
    }

    private static async Task SeedAdminUserAsync(AppDbContext context, IPasswordHasher passwordHasher, ILogger logger, IConfiguration configuration)
    {
        const string adminEmail = "admin@gasapp.com";

        var exists = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email.Value == adminEmail);

        if (exists) return;

        var adminPassword = configuration["Seed:AdminPassword"];
        if (string.IsNullOrWhiteSpace(adminPassword))
            throw new InvalidOperationException("La variable de configuración 'Seed:AdminPassword' es requerida para crear el usuario admin inicial.");

        var hashed = passwordHasher.Hash(adminPassword);
        var user = User.Create(
            new Email(adminEmail),
            hashed,
            "Administrador",
            "GasApp",
            UserRole.Admin,
            phone: null);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        logger.LogInformation("Seed: usuario admin creado — {Email}", adminEmail);
    }
}
