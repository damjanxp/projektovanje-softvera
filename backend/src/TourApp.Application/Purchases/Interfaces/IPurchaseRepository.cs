using TourApp.Domain.Purchases;

namespace TourApp.Application.Purchases.Interfaces;

public interface IPurchaseRepository
{
    Task AddAsync(Purchase purchase);
    Task<Purchase?> GetByIdAsync(Guid id);
    Task<IEnumerable<Purchase>> GetByTouristAsync(Guid touristId);
    Task SaveChangesAsync();
}
