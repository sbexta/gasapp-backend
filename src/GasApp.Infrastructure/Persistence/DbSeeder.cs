using GasApp.Application.Common.Interfaces;
using GasApp.Domain.Entities.Users;
using GasApp.Domain.Enums;
using GasApp.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GasApp.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        await context.Database.MigrateAsync();

        await SeedAdminUserAsync(context, passwordHasher, logger);
    }

    private static async Task SeedAdminUserAsync(AppDbContext context, IPasswordHasher passwordHasher, ILogger logger)
    {
        const string adminEmail = "admin@gasapp.com";

        var exists = await context.Users
            .IgnoreQueryFilters()
            .AnyAsync(u => u.Email.Value == adminEmail);

        if (exists) return;

        var hashed = passwordHasher.Hash("Admin1234!");
        var user = User.Create(
            new Email(adminEmail),
            hashed,
            "Administrador",
            "GasApp",
            UserRole.Admin,
            phone: null);

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        logger.LogInformation("Seed: usuario admin creado — {Email} / Admin1234!", adminEmail);
    }
}
