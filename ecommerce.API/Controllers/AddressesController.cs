using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Address;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/addresses")]
public class AddressesController(IAddressService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Addresses fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("by-user/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
    {
        var result = await service.GetByUserIdAsync(userId, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Addresses fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("by-merchant/{merchantId}")]
    public async Task<IActionResult> GetByMerchantId(int merchantId, CancellationToken cancellationToken)
    {
        var result = await service.GetByMerchantIdAsync(merchantId, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Addresses fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Address fetched successfully."))
            : result.ToProblem();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAddressRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Address created successfully."))
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAddressRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Address updated successfully."))
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Address deleted successfully."))
            : result.ToProblem();
    }
}
