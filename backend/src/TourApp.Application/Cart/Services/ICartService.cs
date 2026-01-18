using TourApp.Application.Cart.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Cart.Services;

public interface ICartService
{
    Task<ApiResponse<CartDto>> AddToCartAsync(Guid touristId, Guid tourId);
    Task<ApiResponse<CartDto>> RemoveFromCartAsync(Guid touristId, Guid tourId);
    Task<ApiResponse<CartDto>> GetCartAsync(Guid touristId);
    void ClearCart(Guid touristId);
    List<CartItemDto> GetCartItems(Guid touristId);
}
