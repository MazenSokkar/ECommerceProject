using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Banners;
using ecommerce.Contracts.Errors;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;

namespace ecommerce.Infrastructure.Services;

public class BannerService(IBannerRepository repository, IUnitOfWork unitOfWork) : IBannerService
{
    public async Task<Result<IEnumerable<BannerResponse>>> GetAllAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var banners = await repository.GetAllAsync();
        
        var filtered = includeInactive 
            ? banners 
            : banners.Where(b => b.IsActive);
            
        var response = filtered.OrderBy(b => b.DisplayOrder).Select(MapToResponse);
        return Result.Success(response);
    }

    public async Task<Result<BannerResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var banner = await repository.GetByIdAsync(id, cancellationToken);
        if (banner is null)
            return Result.Failure<BannerResponse>(BannerErrors.NotFound);

        return Result.Success(MapToResponse(banner));
    }

    public async Task<Result<BannerResponse>> CreateAsync(CreateBannerRequest request, CancellationToken cancellationToken = default)
    {
        await ShiftOrdersAsync(request.DisplayOrder, null, null, cancellationToken);

        var banner = new Banner
        {
            Title = request.Title,
            Content = request.Content,
            ImageUrl = request.ImageUrl,
            LinkUrl = request.LinkUrl ?? string.Empty,
            IsActive = request.IsActive,
            DisplayOrder = request.DisplayOrder
        };

        await repository.AddAsync(banner, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(MapToResponse(banner));
    }

    public async Task<Result<BannerResponse>> UpdateAsync(int id, UpdateBannerRequest request, CancellationToken cancellationToken = default)
    {
        var banner = await repository.GetByIdAsync(id, cancellationToken);
        if (banner is null)
            return Result.Failure<BannerResponse>(BannerErrors.NotFound);

        int oldOrder = banner.DisplayOrder;
        if (oldOrder != request.DisplayOrder)
        {
            await ShiftOrdersAsync(request.DisplayOrder, oldOrder, id, cancellationToken);
        }

        banner.Title = request.Title;
        banner.Content = request.Content;
        banner.ImageUrl = request.ImageUrl;
        banner.LinkUrl = request.LinkUrl ?? string.Empty;
        banner.IsActive = request.IsActive;
        banner.DisplayOrder = request.DisplayOrder;

        await repository.UpdateAsync(banner, cancellationToken);
        await unitOfWork.Complete();

        return Result.Success(MapToResponse(banner));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var banner = await repository.GetByIdAsync(id, cancellationToken);
        if (banner is null)
            return Result.Failure(BannerErrors.NotFound);

        int oldOrder = banner.DisplayOrder;
        await repository.DeleteAsync(id, cancellationToken);

        // Shift elements after the deleted one up (decrement order)
        var allBanners = (await repository.GetAllAsync(cancellationToken)).ToList();
        var bannersToShift = allBanners.Where(b => b.DisplayOrder > oldOrder);
        foreach (var b in bannersToShift)
        {
            b.DisplayOrder--;
            await repository.UpdateAsync(b, cancellationToken);
        }

        await unitOfWork.Complete();

        return Result.Success();
    }

    private static BannerResponse MapToResponse(Banner banner)
        => new(banner.Id, banner.Title, banner.Content, banner.ImageUrl, banner.LinkUrl, banner.IsActive, banner.DisplayOrder);

    private async Task ShiftOrdersAsync(int newOrder, int? oldOrder, int? currentBannerId, CancellationToken cancellationToken)
    {
        var allBanners = (await repository.GetAllAsync(cancellationToken))
            .Where(b => currentBannerId == null || b.Id != currentBannerId.Value)
            .ToList();

        if (oldOrder is null)
        {
            var bannersToShift = allBanners.Where(b => b.DisplayOrder >= newOrder);
            foreach (var b in bannersToShift)
            {
                b.DisplayOrder++;
                await repository.UpdateAsync(b, cancellationToken);
            }
        }
        else if (newOrder != oldOrder)
        {
            if (newOrder < oldOrder)
            {
                // Moving up (e.g. 4 to 2) -> shift elements between 2 and 3 down (increment)
                var bannersToShift = allBanners.Where(b => b.DisplayOrder >= newOrder && b.DisplayOrder < oldOrder);
                foreach (var b in bannersToShift)
                {
                    b.DisplayOrder++;
                    await repository.UpdateAsync(b, cancellationToken);
                }
            }
            else
            {
                // Moving down (e.g. 2 to 4) -> shift elements between 3 and 4 up (decrement)
                var bannersToShift = allBanners.Where(b => b.DisplayOrder > oldOrder && b.DisplayOrder <= newOrder);
                foreach (var b in bannersToShift)
                {
                    b.DisplayOrder--;
                    await repository.UpdateAsync(b, cancellationToken);
                }
            }
        }
    }
}
