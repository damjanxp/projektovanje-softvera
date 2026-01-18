using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using TourApp.Application.Cart.DTOs;
using TourApp.Domain.Tours;
using TourApp.Domain.Users;
using TourApp.Shared;

namespace TourApp.Application.Cart.Services;

public class CartService : ICartService
{
    private static readonly ConcurrentDictionary<Guid, List<CartItemDto>> _carts = new();
    private readonly DbContext _dbContext;

    public CartService(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ApiResponse<CartDto>> AddToCartAsync(Guid touristId, Guid tourId)
    {
        var tour = await _dbContext.Set<Tour>().FirstOrDefaultAsync(t => t.Id == tourId);
        if (tour == null)
        {
            return ApiResponse<CartDto>.Fail("TOUR_NOT_FOUND", "Tour not found.");
        }

        if (tour.Status != TourStatus.Published)
        {
            return ApiResponse<CartDto>.Fail("TOUR_NOT_PUBLISHED", "Only published tours can be added to cart.");
        }

        var cart = _carts.GetOrAdd(touristId, _ => new List<CartItemDto>());

        if (cart.Any(item => item.TourId == tourId))
        {
            return ApiResponse<CartDto>.Fail("TOUR_ALREADY_IN_CART", "This tour is already in your cart.");
        }

        cart.Add(new CartItemDto
        {
            TourId = tour.Id,
            Name = tour.Name,
            Price = tour.Price
        });

        return await GetCartAsync(touristId);
    }

    public async Task<ApiResponse<CartDto>> RemoveFromCartAsync(Guid touristId, Guid tourId)
    {
        if (!_carts.TryGetValue(touristId, out var cart))
        {
            return ApiResponse<CartDto>.Fail("CART_EMPTY", "Your cart is empty.");
        }

        var item = cart.FirstOrDefault(i => i.TourId == tourId);
        if (item == null)
        {
            return ApiResponse<CartDto>.Fail("TOUR_NOT_IN_CART", "This tour is not in your cart.");
        }

        cart.Remove(item);

        return await GetCartAsync(touristId);
    }

    public async Task<ApiResponse<CartDto>> GetCartAsync(Guid touristId)
    {
        var cart = _carts.GetOrAdd(touristId, _ => new List<CartItemDto>());

        var tourist = await _dbContext.Set<Tourist>().FirstOrDefaultAsync(t => t.Id == touristId);
        var bonusPoints = tourist?.BonusPoints ?? 0;

        var totalPrice = cart.Sum(item => item.Price);
        var maxBonusDiscount = Math.Min(bonusPoints, (int)totalPrice);

        var cartDto = new CartDto
        {
            Items = cart.ToList(),
            TotalPrice = totalPrice,
            AvailableBonusPoints = bonusPoints,
            MaxBonusDiscount = maxBonusDiscount
        };

        return ApiResponse<CartDto>.Ok(cartDto);
    }

    public void ClearCart(Guid touristId)
    {
        _carts.TryRemove(touristId, out _);
    }

    public List<CartItemDto> GetCartItems(Guid touristId)
    {
        return _carts.TryGetValue(touristId, out var cart) ? cart.ToList() : new List<CartItemDto>();
    }
}
