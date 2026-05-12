using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Wishlists;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/wishlist")]
[Authorize]
public class WishlistController(IWishlistService service) : ControllerBase
{
    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetMyWishlist(CancellationToken cancellationToken)
    {
        var result = await service.GetMyWishlistAsync(GetCurrentUserId(), cancellationToken);
        return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Wishlist fetched successfully."));
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddToWishlistRequest request, CancellationToken cancellationToken)
    {
        var result = await service.AddItemAsync(GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Item added to wishlist successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(int productId, CancellationToken cancellationToken)
    {
        var result = await service.RemoveItemAsync(GetCurrentUserId(), productId, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Item removed from wishlist successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }
}