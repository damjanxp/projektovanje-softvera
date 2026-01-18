using Microsoft.EntityFrameworkCore;
using TourApp.Application.Purchases.Interfaces;
using TourApp.Domain.Purchases;

namespace TourApp.Infrastructure.Persistence.Repositories;

public class PurchaseRepository : IPurchaseRepository
{
    private readonly TourAppDbContext _context;

    public PurchaseRepository(TourAppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Purchase purchase)
    {
        await _context.Purchases.AddAsync(purchase);
    }

    public async Task<Purchase?> GetByIdAsync(Guid id)
    {
        return await _context.Purchases
            .Include(p => p.PurchasedTours)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Purchase>> GetByTouristAsync(Guid touristId)
    {
        return await _context.Purchases
            .Include(p => p.PurchasedTours)
            .Where(p => p.TouristId == touristId)
            .OrderByDescending(p => p.PurchasedAt)
            .ToListAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
