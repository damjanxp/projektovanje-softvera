using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourApp.Application.Auth.Services;
using TourApp.Domain.Purchases;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TourAppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        // Ensure database schema exists
        await context.Database.MigrateAsync();

        await SeedSystemUsersAsync(context, passwordHasher);
        await SeedTouristsAsync(context, passwordHasher);
        await SeedToursAsync(context);
        await SeedPurchasesAsync(context);
    }

    private static async Task SeedSystemUsersAsync(TourAppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.SystemUsers.AnyAsync())
        {
            return;
        }

        var adminId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        var guideId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guide2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

        var adminPasswordHash = passwordHasher.Hash("Admin123!");
        var guidePasswordHash = passwordHasher.Hash("Guide123!");
        var guide2PasswordHash = passwordHasher.Hash("Guide123!");

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

        var guide2 = new SystemUser(
            guide2Id,
            "guide2",
            "guide2@tourapp.com",
            "Second",
            "Guide",
            Role.Guide,
            guide2PasswordHash
        );

        context.SystemUsers.AddRange(admin, guide, guide2);
        await context.SaveChangesAsync();
    }

    private static async Task SeedTouristsAsync(TourAppDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Tourists.AnyAsync())
        {
            return;
        }

        var tourist1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var tourist2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var tourist3Id = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var tourist1PasswordHash = passwordHasher.Hash("Tourist123!");
        var tourist2PasswordHash = passwordHasher.Hash("Tourist123!");
        var tourist3PasswordHash = passwordHasher.Hash("Tourist123!");

        var tourist1 = new Tourist(
            tourist1Id,
            "john_doe",
            "john.doe@example.com",
            "John",
            "Doe",
            tourist1PasswordHash,
            true,
            new[] { Interest.Nature, Interest.Sport }
        );

        var tourist2 = new Tourist(
            tourist2Id,
            "jane_smith",
            "jane.smith@example.com",
            "Jane",
            "Smith",
            tourist2PasswordHash,
            true,
            new[] { Interest.Art, Interest.Food, Interest.Shopping }
        );

        var tourist3 = new Tourist(
            tourist3Id,
            "bob_wilson",
            "bob.wilson@example.com",
            "Bob",
            "Wilson",
            tourist3PasswordHash,
            false,
            new[] { Interest.Nature }
        );

        context.Tourists.AddRange(tourist1, tourist2, tourist3);
        await context.SaveChangesAsync();
    }

    private static async Task SeedToursAsync(TourAppDbContext context)
    {
        if (await context.Tours.AnyAsync())
        {
            return;
        }

        var guideId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var guide2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

        // Published tours by first guide
        var tour1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var tour1 = new Tour(
            tour1Id,
            guideId,
            "Mountain Hiking Adventure",
            "Explore the breathtaking mountain trails with stunning views and fresh air. Perfect for nature lovers and fitness enthusiasts.",
            Difficulty.Hard,
            Interest.Nature,
            150.00m,
            DateTime.UtcNow.AddDays(30)
        );
        tour1.AddKeyPoint(45.4215, -75.6972, "Trail Start", "Beginning of our mountain journey", "https://example.com/images/trail-start.jpg");
        tour1.AddKeyPoint(45.4315, -75.6872, "Mountain Peak", "The highest point with panoramic views", "https://example.com/images/peak.jpg");
        tour1.AddKeyPoint(45.4415, -75.6772, "Forest Rest Area", "Relaxing spot surrounded by pine trees", "https://example.com/images/forest.jpg");
        tour1.Publish();

        var tour2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var tour2 = new Tour(
            tour2Id,
            guideId,
            "City Art Gallery Tour",
            "Discover the finest art galleries in the city center. Learn about famous artists and their masterpieces.",
            Difficulty.Easy,
            Interest.Art,
            75.00m,
            DateTime.UtcNow.AddDays(15)
        );
        tour2.AddKeyPoint(40.7128, -74.0060, "Modern Art Museum", "Contemporary art from around the world", "https://example.com/images/modern-art.jpg");
        tour2.AddKeyPoint(40.7228, -74.0160, "Classical Gallery", "Renaissance and baroque paintings", "https://example.com/images/classical.jpg");
        tour2.Publish();

        // Published tour by second guide
        var tour3Id = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var tour3 = new Tour(
            tour3Id,
            guide2Id,
            "Food Market Experience",
            "Taste the best local cuisine and learn about traditional cooking methods from local vendors.",
            Difficulty.Easy,
            Interest.Food,
            95.00m,
            DateTime.UtcNow.AddDays(20)
        );
        tour3.AddKeyPoint(51.5074, -0.1278, "Central Market", "The heart of local food culture", "https://example.com/images/central-market.jpg");
        tour3.AddKeyPoint(51.5174, -0.1378, "Street Food Alley", "Authentic street food experience", "https://example.com/images/street-food.jpg");
        tour3.AddKeyPoint(51.5274, -0.1478, "Gourmet Restaurant", "Fine dining and wine tasting", "https://example.com/images/restaurant.jpg");
        tour3.Publish();

        // Draft tour (not yet published)
        var tour4Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
        var tour4 = new Tour(
            tour4Id,
            guide2Id,
            "Shopping District Tour",
            "Visit the most popular shopping areas and boutiques in the city.",
            Difficulty.Easy,
            Interest.Shopping,
            50.00m,
            DateTime.UtcNow.AddDays(45)
        );
        tour4.AddKeyPoint(48.8566, 2.3522, "Fashion Boulevard", "Designer stores and luxury brands", "https://example.com/images/fashion.jpg");
        tour4.AddKeyPoint(48.8666, 2.3622, "Local Boutiques", "Unique handmade items", "https://example.com/images/boutiques.jpg");
        // Not published yet - stays as Draft

        context.Tours.AddRange(tour1, tour2, tour3, tour4);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPurchasesAsync(TourAppDbContext context)
    {
        if (await context.Purchases.AnyAsync())
        {
            return;
        }

        var tourist1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var tourist2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");

        var tour1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var tour2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var tour3Id = Guid.Parse("99999999-9999-9999-9999-999999999999");

        // Purchase 1: Tourist1 buys Mountain Hiking tour
        var purchase1Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var purchase1 = new Purchase(
            purchase1Id,
            tourist1Id,
            150.00m,
            0,
            15 // 10% of price as bonus points
        );
        purchase1.AddPurchasedTour(tour1Id, "Mountain Hiking Adventure", 150.00m);

        // Purchase 2: Tourist2 buys Art Gallery and Food Market tours
        var purchase2Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var purchase2 = new Purchase(
            purchase2Id,
            tourist2Id,
            170.00m, // 75 + 95 = 170
            0,
            17
        );
        purchase2.AddPurchasedTour(tour2Id, "City Art Gallery Tour", 75.00m);
        purchase2.AddPurchasedTour(tour3Id, "Food Market Experience", 95.00m);

        // Purchase 3: Tourist1 buys Food Market tour using bonus points
        var purchase3Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var purchase3 = new Purchase(
            purchase3Id,
            tourist1Id,
            85.00m, // Used 10 bonus points (95 - 10)
            10,
            8
        );
        purchase3.AddPurchasedTour(tour3Id, "Food Market Experience", 95.00m);

        context.Purchases.AddRange(purchase1, purchase2, purchase3);
        await context.SaveChangesAsync();
    }
}