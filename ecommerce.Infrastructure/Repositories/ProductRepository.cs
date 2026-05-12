using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories;

public class ProductRepository(AppDbContext context) : IProductRepository
{
    public async Task<Product?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
        => await context.Products
            .Include(p => p.Merchant)
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<(IEnumerable<Product> Items, int Total)> GetAllAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        string sortBy,
        CancellationToken cancellationToken = default)
    {
        var query = context.Products
            .Include(p => p.Merchant)
            .Include(p => p.Category)
            .Include(p => p.Images)
           .Include(p => p.Reviews)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search) ||
                                    (p.Description != null && p.Description.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId);

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice);

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderByDescending(p => p.CreatedOn)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IEnumerable<Product>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken = default)
        => await context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Where(p => p.MerchantId == merchantId)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        => await context.Products.AddAsync(product, cancellationToken);

    public void Update(Product product)
        => context.Products.Update(product);

    public void Delete(Product product)
        => product.Deleted = true;
}