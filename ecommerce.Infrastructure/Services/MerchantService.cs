using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Errors;
using ecommerce.Contracts.Sellers;
using ecommerce.Core.Entities;
using ecommerce.Core.IRepositories;
using ecommerce.Core.IServices;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace ecommerce.Infrastructure.Services
{
    public class MerchantService(
    IMerchantRepository repository,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IMerchantService
    {
        public async Task<Result<IEnumerable<MerchantResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var Merchants = await repository.GetAllAsync(cancellationToken);
            return Result.Success(mapper.Map<IEnumerable<MerchantResponse>>(Merchants));
        }

        public async Task<Result<MerchantResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var Merchant = await repository.FindByIdAsync(id, cancellationToken);
            if (Merchant is null)
                return Result.Failure<MerchantResponse>(MerchantErrors.NotFound);

            return Result.Success(mapper.Map<MerchantResponse>(Merchant));

        }

        public async Task<Result<MerchantResponse>> RegisterAsync(int userId, CreateMerchantRequest request, CancellationToken cancellationToken = default)
        {
            var Merchant = await repository.ExistsByUserIdAsync(userId,cancellationToken);
            if (Merchant)
                return Result.Failure<MerchantResponse>(MerchantErrors.AlreadyRegistered);

            var seller = mapper.Map<Merchant>(request);
            seller.UserId = userId;
            seller.Status = "pending";

            await repository.AddAsync(seller, cancellationToken);
            await unitOfWork.Complete();

            return Result.Success(mapper.Map<MerchantResponse>(seller));

        }

        public async Task<Result<MerchantResponse>> UpdateProfileAsync(int userId, UpdateMerchantRequest request, CancellationToken cancellationToken = default)
        {

            var Mechant = await repository.FindByUserIdAsync(userId,cancellationToken);
            if (Mechant is null)
                return Result.Failure<MerchantResponse>(MerchantErrors.NotFound);

            mapper.Map(request, Mechant);
            repository.Update(Mechant);
            await unitOfWork.Complete();
            return Result.Success(mapper.Map<MerchantResponse>(Mechant));


        }

        public async Task<Result<MerchantResponse>> UpdateStatusAsync(int id, UpdateMerchantStatusRequest request, CancellationToken cancellationToken = default)
        {
            var marchant = await repository.FindByIdAsync(id,cancellationToken);
            if (marchant is null)
                return Result.Failure<MerchantResponse>(MerchantErrors.NotFound);

            marchant.Status = request.Status.ToLower();
            repository.Update(marchant);
            await unitOfWork.Complete();

            return Result.Success(mapper.Map<MerchantResponse>(marchant));

        }
    }
}
