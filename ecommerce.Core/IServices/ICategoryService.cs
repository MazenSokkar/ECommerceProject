using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Categories;

namespace ecommerce.Core.IServices
{
    public interface ICategoryService
    {
        Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<CategoryResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<CategoryResponse>> CreateAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<Result<CategoryResponse>> UpdateAsync(int id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }

}

