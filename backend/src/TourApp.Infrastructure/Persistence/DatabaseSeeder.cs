using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourApp.Application.Auth.Services;
using TourApp.Domain.Users;

namespace TourApp.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TourAppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await SeedSystemUsersAsync(context, passwordHasher);
    }

    private static async Task SeedSystemUsersAsync(TourAppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.SystemUsers.AnyAsync())
        {
            return;
        }

        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var guideId = Guid.Parse("22222222-2222-2222-2222-222222222222");

        var adminPasswordHash = passwordHasher.Hash("Admin123!");
        var guidePasswordHash = passwordHasher.Hash("Guide123!");

        var admin = new SystemUser(
            adminId,
            "admin",
            "admin@tourapp.com",
            "System",
            "Administrator",
            Role.Admin,
            adminPasswordHash
        );

        var guide = new SystemUser(
            guideId,
            "guide",
            "guide@tourapp.com",
            "Tour",
            "Guide",
            Role.Guide,
            guidePasswordHash
        );

        context.SystemUsers.AddRange(admin, guide);
        await context.SaveChangesAsync();
    }
}
