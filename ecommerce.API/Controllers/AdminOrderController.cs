using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Order;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = DefaultRoles.Admin)]
public class AdminOrderController(IOrderService orderService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await orderService.GetAllOrdersAsync(ct);
        return Ok(result.Value);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await orderService.GetOrderByIdAdminAsync(id, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request, CancellationToken ct)
    {
        var result = await orderService.UpdateOrderStatusAsync(id, request, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error.Description);
    }
}