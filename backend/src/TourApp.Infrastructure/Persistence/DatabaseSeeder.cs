using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TourApp.Application.Auth.Services;
using TourApp.Domain.Problems;
using TourApp.Domain.Purchases;
using TourApp.Domain.Ratings;
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

        // TEMPORARY: Drop and recreate database for fresh seed data
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await SeedSystemUsersAsync(context, passwordHasher);
        await SeedTouristsAsync(context, passwordHasher);
        await SeedToursAsync(context);
        await SeedPurchasesAsync(context);
        await SeedRatingsAndProblemsAsync(context);
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

        // Published tours by first guide (upcoming)
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

        // Published tour by second guide (upcoming)
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

        // Past tours (already completed) - for testing ratings and problems
        var tour5Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var tour5 = new Tour(
            tour5Id,
            guideId,
            "Historic City Walk",
            "Explore the rich history of our city through its ancient streets and monuments.",
            Difficulty.Easy,
            Interest.Art,
            60.00m,
            DateTime.UtcNow.AddDays(-6) // Completed 6 days ago
        );
        tour5.AddKeyPoint(41.9028, 12.4964, "Ancient Forum", "Where history comes alive", "https://example.com/images/forum.jpg");
        tour5.AddKeyPoint(41.9128, 12.5064, "Old Cathedral", "Beautiful medieval architecture", "https://example.com/images/cathedral.jpg");
        tour5.Publish();

        var tour6Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");
        var tour6 = new Tour(
            tour6Id,
            guide2Id,
            "Sunset Beach Walk",
            "Enjoy a peaceful evening walk along the beautiful coastline with breathtaking sunset views.",
            Difficulty.Easy,
            Interest.Nature,
            45.00m,
            DateTime.UtcNow.AddDays(-6.5) // Completed 6.5 days ago (within 7-day rating window)
        );
        tour6.AddKeyPoint(34.0522, -118.2437, "Beach Entrance", "Starting point of our coastal journey", "https://example.com/images/beach-start.jpg");
        tour6.AddKeyPoint(34.0622, -118.2537, "Rocky Overlook", "Perfect spot for sunset photos", "https://example.com/images/overlook.jpg");
        tour6.AddKeyPoint(34.0722, -118.2637, "Lighthouse", "Historic lighthouse with ocean views", "https://example.com/images/lighthouse.jpg");
        tour6.Publish();

        var tour7Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
        var tour7 = new Tour(
            tour7Id,
            guideId,
            "Wine Tasting Tour",
            "Visit local vineyards and sample the finest wines from the region.",
            Difficulty.Easy,
            Interest.Food,
            120.00m,
            DateTime.UtcNow.AddDays(-5) // Completed 5 days ago
        );
        tour7.AddKeyPoint(38.2975, -122.2869, "Vineyard Estate", "First winery on our tour", "https://example.com/images/vineyard1.jpg");
        tour7.AddKeyPoint(38.3075, -122.2969, "Cellar Experience", "Underground wine cellars", "https://example.com/images/cellar.jpg");
        tour7.AddKeyPoint(38.3175, -122.3069, "Gourmet Pairing", "Wine and cheese pairing session", "https://example.com/images/pairing.jpg");
        tour7.Publish();

        var tour8Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
        var tour8 = new Tour(
            tour8Id,
            guide2Id,
            "Extreme Sports Day",
            "Adrenaline-packed day with rock climbing, zip-lining, and more!",
            Difficulty.Hard,
            Interest.Sport,
            180.00m,
            DateTime.UtcNow.AddDays(-3) // Completed 3 days ago
        );
        tour8.AddKeyPoint(37.8651, -119.5383, "Base Camp", "Gear up and safety briefing", "https://example.com/images/basecamp.jpg");
        tour8.AddKeyPoint(37.8751, -119.5483, "Climbing Wall", "Vertical rock climbing challenge", "https://example.com/images/climbing.jpg");
        tour8.AddKeyPoint(37.8851, -119.5583, "Zip Line Course", "Fly through the canopy", "https://example.com/images/zipline.jpg");
        tour8.Publish();

        var tour9Id = Guid.Parse("b4444444-4444-4444-4444-444444444444");
        var tour9 = new Tour(
            tour9Id,
            guideId,
            "Photography Workshop Tour",
            "Learn photography techniques while exploring scenic locations with a professional photographer.",
            Difficulty.Medium,
            Interest.Art,
            85.00m,
            DateTime.UtcNow.AddDays(-4) // Completed 4 days ago
        );
        tour9.AddKeyPoint(40.7589, -73.9851, "Urban Landscape", "Capture the city skyline", "https://example.com/images/urban.jpg");
        tour9.AddKeyPoint(40.7689, -73.9951, "Street Photography", "Candid moments in the city", "https://example.com/images/street.jpg");
        tour9.AddKeyPoint(40.7789, -74.0051, "Golden Hour Spot", "Perfect lighting for portraits", "https://example.com/images/golden-hour.jpg");
        tour9.Publish();

        context.Tours.AddRange(tour1, tour2, tour3, tour4, tour5, tour6, tour7, tour8, tour9);
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
        var tourist3Id = Guid.Parse("66666666-6666-6666-6666-666666666666");

        var tour1Id = Guid.Parse("77777777-7777-7777-7777-777777777777");
        var tour2Id = Guid.Parse("88888888-8888-8888-8888-888888888888");
        var tour3Id = Guid.Parse("99999999-9999-9999-9999-999999999999");
        var tour5Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
        var tour6Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");
        var tour7Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
        var tour8Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
        var tour9Id = Guid.Parse("b4444444-4444-4444-4444-444444444444");

        // Purchase 1: Tourist1 buys Mountain Hiking tour (upcoming)
        var purchase1Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
        var purchase1 = new Purchase(
            purchase1Id,
            tourist1Id,
            150.00m,
            0,
            15 // 10% of price as bonus points
        );
        purchase1.AddPurchasedTour(tour1Id, "Mountain Hiking Adventure", 150.00m);

        // Purchase 2: Tourist2 buys Art Gallery and Food Market tours (upcoming)
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

        // Purchase 3: Tourist1 buys Food Market tour using bonus points (upcoming)
        var purchase3Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
        var purchase3 = new Purchase(
            purchase3Id,
            tourist1Id,
            85.00m, // Used 10 bonus points (95 - 10)
            10,
            8
        );
        purchase3.AddPurchasedTour(tour3Id, "Food Market Experience", 95.00m);

        // Purchase 4: Tourist1 bought the Historic City Walk (past tour)
        var purchase4Id = Guid.Parse("ffffffff-ffff-ffff-ffff-ffffffffffff");
        var purchase4 = new Purchase(
            purchase4Id,
            tourist1Id,
            60.00m,
            0,
            6
        );
        purchase4.AddPurchasedTour(tour5Id, "Historic City Walk", 60.00m);

        // Purchase 5: Tourist2 bought the Historic City Walk (past tour)
        var purchase5Id = Guid.Parse("11111111-2222-3333-4444-555555555555");
        var purchase5 = new Purchase(
            purchase5Id,
            tourist2Id,
            60.00m,
            0,
            6
        );
        purchase5.AddPurchasedTour(tour5Id, "Historic City Walk", 60.00m);

        // Purchase 6: Tourist1 bought Sunset Beach Walk (past tour)
        var purchase6Id = Guid.Parse("c1111111-1111-1111-1111-111111111111");
        var purchase6 = new Purchase(
            purchase6Id,
            tourist1Id,
            45.00m,
            0,
            4
        );
        purchase6.AddPurchasedTour(tour6Id, "Sunset Beach Walk", 45.00m);

        // Purchase 7: Tourist2 bought Wine Tasting Tour (past tour)
        var purchase7Id = Guid.Parse("c2222222-2222-2222-2222-222222222222");
        var purchase7 = new Purchase(
            purchase7Id,
            tourist2Id,
            120.00m,
            0,
            12
        );
        purchase7.AddPurchasedTour(tour7Id, "Wine Tasting Tour", 120.00m);

        // Purchase 8: Tourist3 bought Wine Tasting Tour (past tour)
        var purchase8Id = Guid.Parse("c3333333-3333-3333-3333-333333333333");
        var purchase8 = new Purchase(
            purchase8Id,
            tourist3Id,
            120.00m,
            0,
            12
        );
        purchase8.AddPurchasedTour(tour7Id, "Wine Tasting Tour", 120.00m);

        // Purchase 9: Tourist1 bought Extreme Sports Day (past tour)
        var purchase9Id = Guid.Parse("c4444444-4444-4444-4444-444444444444");
        var purchase9 = new Purchase(
            purchase9Id,
            tourist1Id,
            180.00m,
            0,
            18
        );
        purchase9.AddPurchasedTour(tour8Id, "Extreme Sports Day", 180.00m);

        // Purchase 10: Tourist2 bought Photography Workshop Tour (past tour)
        var purchase10Id = Guid.Parse("c5555555-5555-5555-5555-555555555555");
        var purchase10 = new Purchase(
            purchase10Id,
            tourist2Id,
            85.00m,
            0,
            8
        );
        purchase10.AddPurchasedTour(tour9Id, "Photography Workshop Tour", 85.00m);

        // Purchase 11: Tourist3 bought Sunset Beach Walk (past tour)
        var purchase11Id = Guid.Parse("c6666666-6666-6666-6666-666666666666");
        var purchase11 = new Purchase(
            purchase11Id,
            tourist3Id,
            45.00m,
            0,
            4
        );
        purchase11.AddPurchasedTour(tour6Id, "Sunset Beach Walk", 45.00m);

        context.Purchases.AddRange(purchase1, purchase2, purchase3, purchase4, purchase5, purchase6, purchase7, purchase8, purchase9, purchase10, purchase11);
        await context.SaveChangesAsync();
    }

    private static async Task SeedRatingsAndProblemsAsync(TourAppDbContext context)
    {
        // Seed ratings for the past tours
        if (!await context.Ratings.AnyAsync())
        {
            var tourist1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var tourist2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var tourist3Id = Guid.Parse("66666666-6666-6666-6666-666666666666");
            var tour5Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
            var tour6Id = Guid.Parse("b1111111-1111-1111-1111-111111111111");
            var tour7Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
            var tour8Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
            var tour9Id = Guid.Parse("b4444444-4444-4444-4444-444444444444");

            // Store tour dates to avoid timing issues
            var tour5Date = DateTime.UtcNow.AddDays(-6);
            var tour6Date = DateTime.UtcNow.AddDays(-6.5); // Changed from -7 to -6.5 to stay within 7-day window
            var tour7Date = DateTime.UtcNow.AddDays(-5);
            var tour8Date = DateTime.UtcNow.AddDays(-3);
            var tour9Date = DateTime.UtcNow.AddDays(-4);

            // Ratings for Historic City Walk (tour5) - tour was 6 days ago
            var rating1 = new Rating(
                Guid.Parse("f1111111-1111-1111-1111-111111111111"),
                tourist1Id,
                tour5Id,
                5,
                "Amazing experience! The guide was very knowledgeable and the historical sites were breathtaking.",
                tour5Date
            );

            var rating2 = new Rating(
                Guid.Parse("f2222222-2222-2222-2222-222222222222"),
                tourist2Id,
                tour5Id,
                3,
                "Good tour but a bit rushed. Would have liked more time at each location.",
                tour5Date
            );

            // Ratings for Sunset Beach Walk (tour6) - tour was 6.5 days ago (within 7-day window)
            var rating3 = new Rating(
                Guid.Parse("f3333333-3333-3333-3333-333333333333"),
                tourist1Id,
                tour6Id,
                4,
                "Beautiful sunset views! Very relaxing and peaceful.",
                tour6Date
            );

            var rating4 = new Rating(
                Guid.Parse("f4444444-4444-4444-4444-444444444444"),
                tourist3Id,
                tour6Id,
                5,
                "Perfect evening activity. The guide was friendly and the beach was stunning.",
                tour6Date
            );

            // Ratings for Wine Tasting Tour (tour7) - tour was 5 days ago
            var rating5 = new Rating(
                Guid.Parse("f5555555-5555-5555-5555-555555555555"),
                tourist2Id,
                tour7Id,
                5,
                "Exceptional wine selection and the guide's knowledge about the wines was impressive!",
                tour7Date
            );

            var rating6 = new Rating(
                Guid.Parse("f6666666-6666-6666-6666-666666666666"),
                tourist3Id,
                tour7Id,
                4,
                "Great experience, learned a lot about wine making.",
                tour7Date
            );

            // Rating for Extreme Sports Day (tour8) - tour was 3 days ago
            var rating7 = new Rating(
                Guid.Parse("f7777777-7777-7777-7777-777777777777"),
                tourist1Id,
                tour8Id,
                5,
                "Adrenaline rush! The safety measures were excellent and the guide was very professional.",
                tour8Date
            );

            // Rating for Photography Workshop Tour (tour9) - tour was 4 days ago
            var rating8 = new Rating(
                Guid.Parse("f8888888-8888-8888-8888-888888888888"),
                tourist2Id,
                tour9Id,
                4,
                "Learned some great photography techniques. The locations were well chosen.",
                tour9Date
            );

            context.Ratings.AddRange(rating1, rating2, rating3, rating4, rating5, rating6, rating7, rating8);
            await context.SaveChangesAsync();
        }

        // Seed problems
        if (!await context.Problems.AnyAsync())
        {
            var tourist1Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
            var tourist2Id = Guid.Parse("55555555-5555-5555-5555-555555555555");
            var tour5Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");
            var tour7Id = Guid.Parse("b2222222-2222-2222-2222-222222222222");
            var tour8Id = Guid.Parse("b3333333-3333-3333-3333-333333333333");
            var guideId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var guide2Id = Guid.Parse("33333333-3333-3333-3333-333333333333");

            // Problem 1: Historic City Walk - Pending status
            var problem1 = new Problem(
                Guid.Parse("a1111111-1111-1111-1111-111111111111"),
                tourist1Id,
                tour5Id,
                "Meeting point was unclear",
                "The description said to meet at the main entrance, but there are multiple entrances. I almost missed the tour start."
            );

            // Problem 2: Historic City Walk - InReview status
            var problem2 = new Problem(
                Guid.Parse("a2222222-2222-2222-2222-222222222222"),
                tourist2Id,
                tour5Id,
                "Tour duration was longer than advertised",
                "The tour was supposed to be 3 hours but it lasted 4.5 hours. I had other plans afterwards."
            );
            var event1 = problem2.SendToReview(guideId);

            // Problem 3: Wine Tasting Tour - Pending status
            var problem3 = new Problem(
                Guid.Parse("a3333333-3333-3333-3333-333333333333"),
                tourist2Id,
                tour7Id,
                "Too many people in the group",
                "The group was too large which made it difficult to hear the guide and enjoy the wine tasting properly."
            );

            // Problem 4: Extreme Sports Day - InReview status
            var problem4 = new Problem(
                Guid.Parse("a4444444-4444-4444-4444-444444444444"),
                tourist1Id,
                tour8Id,
                "Equipment was not properly fitted",
                "My harness was loose and I had to wait 15 minutes for it to be adjusted properly. This was concerning for a safety-critical activity."
            );
            var event2 = problem4.SendToReview(guide2Id);

            context.Problems.AddRange(problem1, problem2, problem3, problem4);
            context.ProblemEvents.AddRange(event1, event2);
            await context.SaveChangesAsync();
        }
    }
}