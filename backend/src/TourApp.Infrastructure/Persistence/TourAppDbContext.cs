using Microsoft.EntityFrameworkCore;
using TourApp.Domain.Problems;
using TourApp.Domain.Purchases;
using TourApp.Domain.Ratings;
using TourApp.Domain.Security;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;

namespace TourApp.Infrastructure.Persistence;

public class TourAppDbContext : DbContext
{
    public DbSet<Tourist> Tourists { get; set; }
    public DbSet<TouristInterest> TouristInterests { get; set; }
    public DbSet<SystemUser> SystemUsers { get; set; }
    public DbSet<LoginAttemptTracker> LoginAttemptTrackers { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<KeyPoint> KeyPoints { get; set; }
    public DbSet<Purchase> Purchases { get; set; }
    public DbSet<PurchasedTour> PurchasedTours { get; set; }
    public DbSet<Rating> Ratings { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemStatusChangedEvent> ProblemEvents { get; set; }

    public TourAppDbContext(DbContextOptions<TourAppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureTourist(modelBuilder);
        ConfigureTouristInterest(modelBuilder);
        ConfigureSystemUser(modelBuilder);
        ConfigureLoginAttemptTracker(modelBuilder);
        ConfigureTour(modelBuilder);
        ConfigureKeyPoint(modelBuilder);
        ConfigurePurchase(modelBuilder);
        ConfigurePurchasedTour(modelBuilder);
        ConfigureRating(modelBuilder);
        ConfigureProblem(modelBuilder);
        ConfigureProblemEvent(modelBuilder);
    }

    private static void ConfigureTourist(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tourist>(entity =>
        {
            entity.ToTable("tourists");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(t => t.Username)
                .IsUnique();

            entity.Property(t => t.Email)
                .IsRequired()
                .HasMaxLength(120);

            entity.HasIndex(t => t.Email)
                .IsUnique();

            entity.Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(t => t.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(t => t.WantsRecommendations)
                .IsRequired();

            entity.Property(t => t.BonusPoints)
                .IsRequired()
                .HasDefaultValue(0);

            entity.HasMany(t => t.Interests)
                .WithOne()
                .HasForeignKey(ti => ti.TouristId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureTouristInterest(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TouristInterest>(entity =>
        {
            entity.ToTable("tourist_interests");

            entity.HasKey(ti => new { ti.TouristId, ti.Interest });

            entity.Property(ti => ti.Interest)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);
        });
    }

    private static void ConfigureSystemUser(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemUser>(entity =>
        {
            entity.ToTable("system_users");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(u => u.Username)
                .IsUnique();

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(120);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(80);

            entity.Property(u => u.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);
        });
    }

    private static void ConfigureLoginAttemptTracker(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoginAttemptTracker>(entity =>
        {
            entity.ToTable("login_attempt_trackers");

            entity.HasKey(l => l.Id);

            entity.Property(l => l.Username)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasIndex(l => l.Username)
                .IsUnique();

            entity.Property(l => l.FailedCount)
                .IsRequired();

            entity.Property(l => l.IsBlocked)
                .IsRequired();

            entity.Property(l => l.BlockCount)
                .IsRequired();

            entity.Property(l => l.LastFailedAt);

            entity.Property(l => l.BlockedAt);
                    });
                }

                private static void ConfigureTour(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<Tour>(entity =>
                    {
                        entity.ToTable("tours");

                        entity.HasKey(t => t.Id);

                        entity.Property(t => t.GuideId)
                            .IsRequired();

                        entity.HasIndex(t => t.GuideId);

                        entity.Property(t => t.Name)
                            .IsRequired()
                            .HasMaxLength(200);

                        entity.Property(t => t.Description)
                            .IsRequired()
                            .HasMaxLength(2000);

                        entity.Property(t => t.Difficulty)
                            .IsRequired()
                            .HasConversion<string>()
                            .HasMaxLength(20);

                        entity.Property(t => t.Category)
                            .IsRequired()
                            .HasConversion<string>()
                            .HasMaxLength(20);

                        entity.Property(t => t.Price)
                            .IsRequired()
                            .HasPrecision(18, 2);

                        entity.Property(t => t.StartDate)
                            .IsRequired();

                        entity.Property(t => t.Status)
                            .IsRequired()
                            .HasConversion<string>()
                            .HasMaxLength(20);

                        entity.HasIndex(t => t.Status);

                        entity.Property(t => t.NeedsReplacement)
                            .IsRequired()
                            .HasDefaultValue(false);

                        entity.Property(t => t.ReplacementRequestedAt);

                        entity.HasIndex(t => t.NeedsReplacement);

                        entity.HasMany(t => t.KeyPoints)
                            .WithOne()
                            .HasForeignKey(kp => kp.TourId)
                            .OnDelete(DeleteBehavior.Cascade);

                        entity.Navigation(t => t.KeyPoints).UsePropertyAccessMode(PropertyAccessMode.Field);
                    });
                }

                private static void ConfigureKeyPoint(ModelBuilder modelBuilder)
                {
                    modelBuilder.Entity<KeyPoint>(entity =>
                    {
                        entity.ToTable("key_points");

                        entity.HasKey(kp => kp.Id);

                        entity.Property(kp => kp.TourId)
                            .IsRequired();

                        entity.Property(kp => kp.Latitude)
                            .IsRequired();

                        entity.Property(kp => kp.Longitude)
                            .IsRequired();

                        entity.Property(kp => kp.Name)
                            .IsRequired()
                            .HasMaxLength(200);

                        entity.Property(kp => kp.Description)
                            .IsRequired()
                            .HasMaxLength(1000);

                        entity.Property(kp => kp.ImageUrl)
                            .IsRequired()
                            .HasMaxLength(500);

                        entity.Property(kp => kp.Order)
                                                    .IsRequired();
                                            });
                                        }

                            private static void ConfigurePurchase(ModelBuilder modelBuilder)
                            {
                                modelBuilder.Entity<Purchase>(entity =>
                                {
                                    entity.ToTable("purchases");

                                    entity.HasKey(p => p.Id);

                                    entity.Property(p => p.TouristId)
                                        .IsRequired();

                                    entity.HasIndex(p => p.TouristId);

                                    entity.Property(p => p.PurchasedAt)
                                        .IsRequired();

                                    entity.Property(p => p.TotalPrice)
                                        .IsRequired()
                                        .HasPrecision(18, 2);

                                    entity.Property(p => p.BonusPointsUsed)
                                        .IsRequired();

                                    entity.Property(p => p.BonusPointsEarned)
                                        .IsRequired();

                                    entity.HasMany(p => p.PurchasedTours)
                                        .WithOne()
                                        .HasForeignKey(pt => pt.PurchaseId)
                                        .OnDelete(DeleteBehavior.Cascade);

                                    entity.Navigation(p => p.PurchasedTours).UsePropertyAccessMode(PropertyAccessMode.Field);
                                });
                            }

                            private static void ConfigurePurchasedTour(ModelBuilder modelBuilder)
                            {
                                modelBuilder.Entity<PurchasedTour>(entity =>
                                {
                                    entity.ToTable("purchased_tours");

                                    entity.HasKey(pt => pt.Id);

                                    entity.Property(pt => pt.PurchaseId)
                                        .IsRequired();

                                    entity.Property(pt => pt.TourId)
                                        .IsRequired();

                                    entity.Property(pt => pt.TourName)
                                        .IsRequired()
                                        .HasMaxLength(200);

                                    entity.Property(pt => pt.PriceAtPurchase)
                                        .IsRequired()
                                        .HasPrecision(18, 2);
                                });
                            }

    private static void ConfigureRating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>(entity =>
        {
            entity.ToTable("ratings");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.TouristId)
                .IsRequired();

            entity.HasIndex(r => r.TouristId);

            entity.Property(r => r.TourId)
                .IsRequired();

            entity.HasIndex(r => r.TourId);

            entity.HasIndex(r => new { r.TouristId, r.TourId })
                .IsUnique();

            entity.Property(r => r.Score)
                .IsRequired();

            entity.Property(r => r.Comment)
                .HasMaxLength(2000);

            entity.Property(r => r.CreatedAt)
                .IsRequired();
        });
    }

    private static void ConfigureProblem(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Problem>(entity =>
        {
            entity.ToTable("problems");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.TouristId)
                .IsRequired();

            entity.HasIndex(p => p.TouristId);

            entity.Property(p => p.TourId)
                .IsRequired();

            entity.HasIndex(p => p.TourId);

            entity.Property(p => p.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(2000);

            entity.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.HasIndex(p => p.Status);

            entity.Property(p => p.CreatedAt)
                .IsRequired();

            entity.HasMany(p => p.Events)
                .WithOne()
                .HasForeignKey(e => e.ProblemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Navigation(p => p.Events).UsePropertyAccessMode(PropertyAccessMode.Field);
        });
    }

    private static void ConfigureProblemEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProblemStatusChangedEvent>(entity =>
        {
            entity.ToTable("problem_events");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.ProblemId)
                .IsRequired();

            entity.HasIndex(e => e.ProblemId);

            entity.Property(e => e.OldStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.NewStatus)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(e => e.ChangedAt)
                .IsRequired();

            entity.Property(e => e.ChangedByRole)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.ChangedByUserId)
                .IsRequired();
        });
    }
}
