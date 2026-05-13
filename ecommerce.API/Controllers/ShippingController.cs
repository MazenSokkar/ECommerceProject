using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Core.Entities.salah_entities;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShippingController(IShippingService shippingService) : ControllerBase
    {
        [HttpPost("{orderId:int}")]
        [Authorize(Roles = DefaultRoles.Admin)]
        public async Task<IActionResult> Create(int orderId, CancellationToken cancellationToken)
        {
            var result = await shippingService.CreateShipmentAsync(orderId, cancellationToken);
            return result.IsSuccess ? Ok() : BadRequest(result.Error.Description);
        }

        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> Get(int orderId, CancellationToken cancellationToken)
        {
            var result = await shippingService.GetByOrderIdAsync(orderId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error.Description);
        }

        [HttpPut("{orderId:int}/status")]
        [Authorize(Roles = DefaultRoles.Admin)]
        public async Task<IActionResult> UpdateStatus(
            int orderId,
            [FromQuery] ShippingStatus status,
            CancellationToken cancellationToken)
        {
            var result = await shippingService.UpdateStatusAsync(orderId, status, cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result.Error.Description);
        }
    }
}