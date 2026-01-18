using TourApp.Application.Purchases.DTOs;
using TourApp.Shared;

namespace TourApp.Application.Purchases.Services;

public interface IPurchaseService
{
    Task<ApiResponse<PurchaseResultDto>> ConfirmPurchaseAsync(Guid touristId, bool useBonus);
    Task<ApiResponse<List<PurchaseResultDto>>> GetMyPurchasesAsync(Guid touristId);
}
