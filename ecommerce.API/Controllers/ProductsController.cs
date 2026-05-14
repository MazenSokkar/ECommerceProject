using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Products;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ecommerce.Contracts.Abstractions;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductService service) : ControllerBase
{
    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string sortBy = "newest",
        CancellationToken cancellationToken = default)
    {
        var result = await service.GetAllAsync(search, categoryId, minPrice, maxPrice, page, pageSize, sortBy, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Products fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Product fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("my-products")]
    [Authorize(Roles = DefaultRoles.Merchant)]
    public async Task<IActionResult> GetMyProducts(CancellationToken cancellationToken)
    {
        var result = await service.GetMyProductsAsync(GetCurrentUserId(), cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Products fetched successfully."))
            : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Merchant)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Product created successfully."))
            : result.ToProblem();
    }

    [HttpPut("{id}")]
    [Authorize(Roles = DefaultRoles.Merchant)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(GetCurrentUserId(), id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Product updated successfully."))
            : result.ToProblem();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = DefaultRoles.Merchant)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(GetCurrentUserId(), id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Product deleted successfully."))
            : result.ToProblem();
    }

    [HttpPut("{id}/stock")]
    [Authorize(Roles = DefaultRoles.Merchant)]
    public async Task<IActionResult> UpdateStock(int id, [FromBody] UpdateStockRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStockAsync(GetCurrentUserId(), id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Stock updated successfully."))
            : result.ToProblem();
    }
}