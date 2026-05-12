using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Products;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;

namespace ecommerce.Infrastructure.Services;

public class ProductService(
    IProductRepository repository,
    IMerchantRepository merchantRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IProductService
{
    public async Task<Result<ProductListResponse>> GetAllAsync(
        string? search, int? categoryId, decimal? minPrice, decimal? maxPrice,
        int page, int pageSize, string sortBy, CancellationToken cancellationToken = default)
    {
        var (items, total) = await repository.GetAllAsync(
            search, categoryId, minPrice, maxPrice, page, pageSize, sortBy, cancellationToken);

        var response = items.Select(p => new ProductListItemResponse(
            p.Id, p.Name, p.Price, p.Stock, p.IsActive,
            p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0,
            p.Reviews.Count,
            p.Category.Name,
            p.Merchant.StoreName
        ));

        return Result.Success(new ProductListResponse(response, total));
    }

    public async Task<Result<ProductResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.FindByIdAsync(id, cancellationToken);
        if (product is null)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);

        return Result.Success(new ProductResponse(
            product.Id,
            product.MerchantId,
            product.Merchant.StoreName,
            product.CategoryId,
            product.Category.Name,
            product.Name,
            product.Description,
            product.Price,
            product.Stock,
            product.IsActive,
            product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
            product.Reviews.Count,
            product.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl).ToList(),
            product.CreatedOn
        ));
    }

    public async Task<Result<IEnumerable<ProductListItemResponse>>> GetMyProductsAsync(int userId, CancellationToken cancellationToken = default)
    {
        var merchant = await merchantRepository.FindByUserIdAsync(userId, cancellationToken);
        if (merchant is null)
            return Result.Failure<IEnumerable<ProductListItemResponse>>(ProductErrors.MerchantNotFound);

        var products = await repository.GetByMerchantIdAsync(merchant.Id, cancellationToken);

        var response = products.Select(p => new ProductListItemResponse(
            p.Id, p.Name, p.Price, p.Stock, p.IsActive,
            p.Images.FirstOrDefault(i => i.IsPrimary)?.ImageUrl,
            0, 0,
            p.Category.Name,
            merchant.StoreName
        ));

        return Result.Success(response);
    }

    public async Task<Result<ProductResponse>> CreateAsync(int userId, CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        var merchant = await merchantRepository.FindByUserIdAsync(userId, cancellationToken);
        if (merchant is null)
            return Result.Failure<ProductResponse>(ProductErrors.MerchantNotFound);
        if (merchant.Status != "approved")
            return Result.Failure<ProductResponse>(ProductErrors.MerchantNotApproved);

        var category = await categoryRepository.FindByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return Result.Failure<ProductResponse>(ProductErrors.CategoryNotFound);

        var product = mapper.Map<Product>(request);
        product.MerchantId = merchant.Id;

        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.Complete();

        return await GetByIdAsync(product.Id, cancellationToken);
    }

    public async Task<Result<ProductResponse>> UpdateAsync(int userId, int id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var merchant = await merchantRepository.FindByUserIdAsync(userId, cancellationToken);
        if (merchant is null)
            return Result.Failure<ProductResponse>(ProductErrors.MerchantNotFound);

        var product = await repository.FindByIdAsync(id, cancellationToken);
        if (product is null)
            return Result.Failure<ProductResponse>(ProductErrors.NotFound);

        if (product.MerchantId != merchant.Id)
            return Result.Failure<ProductResponse>(ProductErrors.NotOwner);

        mapper.Map(request, product);
        repository.Update(product);
        await unitOfWork.Complete();

        return await GetByIdAsync(product.Id, cancellationToken);
    }

    public async Task<Result> DeleteAsync(int userId, int id, CancellationToken cancellationToken = default)
    {
        var merchant = await merchantRepository.FindByUserIdAsync(userId, cancellationToken);
        if (merchant is null)
            return Result.Failure(ProductErrors.MerchantNotFound);

        var product = await repository.FindByIdAsync(id, cancellationToken);
        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        if (product.MerchantId != merchant.Id)
            return Result.Failure(ProductErrors.NotOwner);

        repository.Delete(product);
        await unitOfWork.Complete();

        return Result.Success();
    }

    public async Task<Result> UpdateStockAsync(int userId, int id, UpdateStockRequest request, CancellationToken cancellationToken = default)
    {
        var merchant = await merchantRepository.FindByUserIdAsync(userId, cancellationToken);
        if (merchant is null)
            return Result.Failure(ProductErrors.MerchantNotFound);

        var product = await repository.FindByIdAsync(id, cancellationToken);
        if (product is null)
            return Result.Failure(ProductErrors.NotFound);

        if (product.MerchantId != merchant.Id)
            return Result.Failure(ProductErrors.NotOwner);

        product.Stock = request.Stock;
        repository.Update(product);
        await unitOfWork.Complete();

        return Result.Success();
    }
}