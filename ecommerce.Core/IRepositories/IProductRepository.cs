using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories;

public interface IProductRepository
{
    Task<Product?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Product> Items, int Total)> GetAllAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        string sortBy,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<Product>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Update(Product product);
    void Delete(Product product);
}