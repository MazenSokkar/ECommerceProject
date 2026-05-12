using ecommerce.Core.Entities;

namespace ecommerce.Core.IRepositories
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Category?> FindByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);

        Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(Category category, CancellationToken cancellationToken = default);
        void Update(Category category);
        void Delete(Category category);
    }
}
