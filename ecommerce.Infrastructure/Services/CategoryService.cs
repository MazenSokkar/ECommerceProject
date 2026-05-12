using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Categories;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;

namespace ecommerce.Infrastructure.Services;

public class CategoryService(
    ICategoryRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : ICategoryService
{
    public async Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await repository.GetAllAsync(cancellationToken);
        return Result.Success(mapper.Map<IEnumerable<CategoryResponse>>(categories));
    }

    public async Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await repository.FindByIdAsync(id, cancellationToken);
        if (category is null)
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);

        return Result.Success(mapper.Map<CategoryResponse>(category));
    }

    public async Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var exists = await repository.ExistsByNameAsync(request.Name, cancellationToken);
        if (exists)
            return Result.Failure<CategoryResponse>(CategoryErrors.DuplicateName);

        var category = mapper.Map<Category>(request);
        category.Slug = request.Name.ToLower().Replace(" ", "-");

        await repository.AddAsync(category, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<CategoryResponse>(category));
    }

    public async Task<Result<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default)
    {
        var category = await repository.FindByIdAsync(id, cancellationToken);
        if (category is null)
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);

        mapper.Map(request, category);
        category.Slug = request.Name.ToLower().Replace(" ", "-");

        repository.Update(category);
        await unitOfWork.Complete();

        return Result.Success(mapper.Map<CategoryResponse>(category));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await repository.FindByIdAsync(id, cancellationToken);
        if (category is null)
            return Result.Failure(CategoryErrors.NotFound);

        var hasProducts = await repository.HasProductsAsync(id, cancellationToken);
        if (hasProducts)
            return Result.Failure(CategoryErrors.HasProducts);

        repository.Delete(category);
        await unitOfWork.Complete();

        return Result.Success();
    }
}