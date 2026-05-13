using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/admin/shipping")]
[Authorize(Roles = DefaultRoles.Admin)]
public class AdminShippingController(IShippingService shippingService) : ControllerBase
{
    [HttpPost("{orderId:int}")]
    public async Task<IActionResult> Create(int orderId, CancellationToken ct)
    {
        var result = await shippingService.CreateShipmentAsync(orderId, ct);
        return result.IsSuccess ? Ok() : BadRequest(result.Error.Description);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> Get(int orderId, CancellationToken ct)
    {
        var result = await shippingService.GetByOrderIdAsync(orderId, ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPut("{orderId:int}/status")]
    public async Task<IActionResult> Update(int orderId, ShippingStatus status, CancellationToken ct)
    {
        var result = await shippingService.UpdateStatusAsync(orderId, status, ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error.Description);
    }
}