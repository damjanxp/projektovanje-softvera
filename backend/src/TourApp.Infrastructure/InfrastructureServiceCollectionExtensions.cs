using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourApp.Application.Admin.Services;
using TourApp.Application.Auth.Services;
using TourApp.Application.Cart.Services;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Application.Purchases.Services;
using TourApp.Application.Tours.Interfaces;
using TourApp.Application.Tours.Services;
using TourApp.Infrastructure.Persistence;
using TourApp.Infrastructure.Persistence.Repositories;
using TourApp.Infrastructure.Services;

namespace TourApp.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<TourAppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<DbContext>(provider => provider.GetRequiredService<TourAppDbContext>());

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<ITourRepository, TourRepository>();
        services.AddScoped<ITourService, TourService>();
        services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
