using System.Security.Claims;
using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Order;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController(IOrderService orderService) : ControllerBase
{
    private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Customer)]

    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderRequest request,
        CancellationToken cancellationToken)
    {
        var result = await orderService.PlaceOrderAsync(GetUserId(), request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error.Description);
    }

    [HttpGet]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> GetMyOrders(CancellationToken cancellationToken)
    {
        var result = await orderService.GetMyOrdersAsync(GetUserId(), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error.Description);
    }

    [HttpGet("{orderId:int}")]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> GetOrderById(
        int orderId,
        CancellationToken cancellationToken)
    {
        var result = await orderService.GetOrderByIdAsync(GetUserId(), orderId, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : Problem(result.Error.Description);
    }

    [HttpDelete("{orderId:int}")]
    [Authorize(Roles = DefaultRoles.Customer)]

    public async Task<IActionResult> CancelOrder(
        int orderId,
        CancellationToken cancellationToken)
    {
        var result = await orderService.CancelOrderAsync(GetUserId(), orderId, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Error.Description);
    }

    // Admin Only
    [HttpPut("{orderId:int}/status")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> UpdateOrderStatus(
        int orderId,
        [FromBody] UpdateOrderStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await orderService.UpdateOrderStatusAsync(orderId, request, cancellationToken);
        return result.IsSuccess ? NoContent() : Problem(result.Error.Description);
    }
}