using Microsoft.EntityFrameworkCore;
using TourApp.Application.Cart.Services;
using TourApp.Application.Purchases.DTOs;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Domain.Purchases;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Application.Purchases.Services;

public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ICartService _cartService;
    private readonly IEmailService _emailService;
    private readonly DbContext _dbContext;

    private const int BonusPointsPerPurchase = 10;

    public PurchaseService(
        IPurchaseRepository purchaseRepository,
        ICartService cartService,
        IEmailService emailService,
        DbContext dbContext)
    {
        _purchaseRepository = purchaseRepository;
        _cartService = cartService;
        _emailService = emailService;
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<PurchaseResultDto>> ConfirmPurchaseAsync(Guid touristId, bool useBonus)
    {
        var cartItems = _cartService.GetCartItems(touristId);
        if (!cartItems.Any())
        {
            return ApiResponse<PurchaseResultDto>.Fail("CART_EMPTY", "Your cart is empty.");
        }

        var tourist = await _dbContext.Set<Tourist>().FirstOrDefaultAsync(t => t.Id == touristId);
        if (tourist == null)
        {
            return ApiResponse<PurchaseResultDto>.Fail("TOURIST_NOT_FOUND", "Tourist not found.");
        }

        var tourIds = cartItems.Select(i => i.TourId).ToList();
        var tours = await _dbContext.Set<Tour>()
            .Where(t => tourIds.Contains(t.Id))
            .ToListAsync();

        foreach (var cartItem in cartItems)
        {
            var tour = tours.FirstOrDefault(t => t.Id == cartItem.TourId);
            if (tour == null)
            {
                return ApiResponse<PurchaseResultDto>.Fail("TOUR_NOT_FOUND", $"Tour '{cartItem.Name}' not found.");
            }
            if (tour.Status != TourStatus.Published)
            {
                return ApiResponse<PurchaseResultDto>.Fail("TOUR_NOT_PUBLISHED", $"Tour '{cartItem.Name}' is no longer available.");
            }
        }

        var originalPrice = cartItems.Sum(i => i.Price);
        var bonusPointsUsed = 0;
        var finalPrice = originalPrice;

        if (useBonus && tourist.BonusPoints > 0)
        {
            bonusPointsUsed = Math.Min(tourist.BonusPoints, (int)originalPrice);
            finalPrice = Math.Max(0, originalPrice - bonusPointsUsed);
            tourist.SpendBonusPoints(bonusPointsUsed);
        }

        var bonusPointsEarned = BonusPointsPerPurchase;
        tourist.AddBonusPoints(bonusPointsEarned);

        var purchase = new Purchase(
            Guid.NewGuid(),
            touristId,
            finalPrice,
            bonusPointsUsed,
            bonusPointsEarned
        );

        foreach (var cartItem in cartItems)
        {
            purchase.AddPurchasedTour(cartItem.TourId, cartItem.Name, cartItem.Price);
        }

        await _purchaseRepository.AddAsync(purchase);
        await _purchaseRepository.SaveChangesAsync();

        _cartService.ClearCart(touristId);

        var result = new PurchaseResultDto
        {
            PurchaseId = purchase.Id,
            OriginalPrice = originalPrice,
            FinalPrice = finalPrice,
            BonusPointsUsed = bonusPointsUsed,
            BonusPointsEarned = bonusPointsEarned,
            TotalBonusPoints = tourist.BonusPoints,
            PurchasedAt = purchase.PurchasedAt,
            Tours = purchase.PurchasedTours.Select(pt => new PurchasedTourDto
            {
                TourId = pt.TourId,
                TourName = pt.TourName,
                PriceAtPurchase = pt.PriceAtPurchase
            }).ToList()
        };

        await _emailService.SendPurchaseConfirmationAsync(tourist.Email, $"{tourist.FirstName} {tourist.LastName}", result);

        return ApiResponse<PurchaseResultDto>.Ok(result);
    }

    public async Task<ApiResponse<List<PurchaseResultDto>>> GetMyPurchasesAsync(Guid touristId)
    {
        var purchases = await _purchaseRepository.GetByTouristAsync(touristId);

        var results = purchases.Select(p => new PurchaseResultDto
        {
            PurchaseId = p.Id,
            OriginalPrice = p.PurchasedTours.Sum(pt => pt.PriceAtPurchase),
            FinalPrice = p.TotalPrice,
            BonusPointsUsed = p.BonusPointsUsed,
            BonusPointsEarned = p.BonusPointsEarned,
            TotalBonusPoints = 0,
            PurchasedAt = p.PurchasedAt,
            Tours = p.PurchasedTours.Select(pt => new PurchasedTourDto
            {
                TourId = pt.TourId,
                TourName = pt.TourName,
                PriceAtPurchase = pt.PriceAtPurchase
            }).ToList()
        }).ToList();

        return ApiResponse<List<PurchaseResultDto>>.Ok(results);
    }
}
