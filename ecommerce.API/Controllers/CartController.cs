using System.Security.Claims;
using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Cart;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class CartController(ICartService cartService) : ControllerBase
{
    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    //private int GetCurrentUserId()
    //       => 2;
    private string GetSessionId()
        => HttpContext.Session.GetString("CartSessionId")
           ?? GenerateAndStoreSessionId();

    private string GenerateAndStoreSessionId()
    {
        var id = Guid.NewGuid().ToString();
        HttpContext.Session.SetString("CartSessionId", id);
        return id;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken cancellationToken)
    {
        var result = await cartService.GetCartAsync(
            GetCurrentUserId(),
            null,
            cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Cart fetched successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpPost("items")]
    [Authorize(Roles = DefaultRoles.Customer)]

    public async Task<IActionResult> AddItem(
        [FromBody] AddToCartRequest request,
        CancellationToken cancellationToken)
    {
        var result = await cartService.AddItemAsync(
            GetCurrentUserId(),
            null,
            request,
            cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Item added to cart successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [Authorize(Roles = DefaultRoles.Customer)]
    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(
        int productId,
        [FromBody] UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var result = await cartService.UpdateItemAsync(
            GetCurrentUserId(),
            null,
            productId,
            request,
            cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Cart item updated successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [Authorize(Roles = DefaultRoles.Customer)]
    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(
        int productId,
        CancellationToken cancellationToken)
    {
        var result = await cartService.RemoveItemAsync(
            GetCurrentUserId(),
            null,
            productId,
            cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Item removed successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [Authorize(Roles = DefaultRoles.Customer)]
    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken cancellationToken)
    {
        var result = await cartService.ClearCartAsync(
            GetCurrentUserId(),
            null,
            cancellationToken);

        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Cart cleared successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }
}