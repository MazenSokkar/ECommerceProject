using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Products;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HomePageController(IProductService productService) : ControllerBase
{
    [HttpGet("best-sellers")]
    public async Task<IActionResult> GetBestSellers([FromQuery] int count = 8, CancellationToken cancellationToken = default)
    {
        var result = await productService.GetBestSellersAsync(count, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Best sellers fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("new-arrivals")]
    public async Task<IActionResult> GetNewArrivals([FromQuery] int count = 8, CancellationToken cancellationToken = default)
    {
        var result = await productService.GetNewArrivalsAsync(count, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "New arrivals fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("featured")]
    public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 8, CancellationToken cancellationToken = default)
    {
        var result = await productService.GetFeaturedProductsAsync(count, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Featured products fetched successfully."))
            : result.ToProblem();
    }
}
