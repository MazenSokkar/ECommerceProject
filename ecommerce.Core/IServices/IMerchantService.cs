using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Sellers;
using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Core.IServices
{
    public interface IMerchantService
    {
        Task<Result<IEnumerable<MerchantResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<MerchantResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Result<MerchantResponse>> RegisterAsync(int userId, CreateMerchantRequest request, CancellationToken cancellationToken = default);
        Task<Result<MerchantResponse>> UpdateProfileAsync (int userId , UpdateMerchantRequest request, CancellationToken cancellationToken = default);
        Task<Result<MerchantResponse>> UpdateStatusAsync(int id , UpdateMerchantStatusRequest request, CancellationToken cancellationToken = default);

    }
}
