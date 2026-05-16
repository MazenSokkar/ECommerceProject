using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Products;

namespace ecommerce.Core.IServices;

public interface IProductService
{
    Task<Result<ProductListResponse>> GetAllAsync(
        string? search,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        int page,
        int pageSize,
        string sortBy,
        CancellationToken cancellationToken = default);

    Task<Result<ProductResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductListItemResponse>>> GetMyProductsAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductListItemResponse>>> GetBestSellersAsync(int count, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductListItemResponse>>> GetNewArrivalsAsync(int count, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<ProductListItemResponse>>> GetFeaturedProductsAsync(int count, CancellationToken cancellationToken = default);
    
    Task<Result<ProductResponse>> CreateAsync(int userId, CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> UpdateAsync(int userId, int id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int userId, int id, CancellationToken cancellationToken = default);
    Task<Result> UpdateStockAsync(int userId, int id, UpdateStockRequest request, CancellationToken cancellationToken = default);
    Task<Result> AdminDeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<Result> AddImageAsync(int productId, AddProductImageRequest request, CancellationToken cancellationToken = default);
}