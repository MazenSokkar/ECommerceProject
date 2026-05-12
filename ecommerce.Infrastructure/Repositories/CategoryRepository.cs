using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Infrastructure.Data;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Repositories
{
    public class CategoryRepository(AppDbContext context) : ICategoryRepository
    {
        public async Task<Category?> FindByIdAsync(int id, CancellationToken cancellationToken = default)

          => await context.Categories
           .Include(c => c.Children)
           .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        => await context.Categories
            .AnyAsync(c => c.Name == name, cancellationToken);
        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default)
                => await context.Categories
            .Include(c => c.Children)
            .Where(c => c.ParentId == null)
            .ToListAsync(cancellationToken);
        public async Task<bool> HasProductsAsync(int id, CancellationToken cancellationToken = default)
                => await context.Products
            .AnyAsync(p => p.CategoryId == id, cancellationToken);


        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        => await context.Categories.AddAsync(category, cancellationToken);
        public void Update(Category category)
        => context.Categories.Update(category);


        public void Delete(Category category)
        => category.Deleted = true;
    }
}
