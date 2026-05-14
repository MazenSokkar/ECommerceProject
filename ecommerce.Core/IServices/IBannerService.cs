using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Banners;

namespace ecommerce.Core.IServices;

public interface IBannerService
{
    Task<Result<IEnumerable<BannerResponse>>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Result<BannerResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<BannerResponse>> CreateAsync(CreateBannerRequest request, CancellationToken cancellationToken = default);
    Task<Result<BannerResponse>> UpdateAsync(int id, UpdateBannerRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
