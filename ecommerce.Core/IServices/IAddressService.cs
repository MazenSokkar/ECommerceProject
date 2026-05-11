using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Address;

namespace ecommerce.Core.IServices;

public interface IAddressService
{
    Task<Result<IEnumerable<AddressResponse>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AddressResponse>>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AddressResponse>>> GetByMerchantIdAsync(int merchantId, CancellationToken cancellationToken = default);
    Task<Result<AddressResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<AddressResponse>> CreateAsync(CreateAddressRequest request, CancellationToken cancellationToken = default);
    Task<Result<AddressResponse>> UpdateAsync(int id, UpdateAddressRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
