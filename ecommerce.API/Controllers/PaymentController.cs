using ecommerce.Contracts.Payment;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentController(IPaymentService paymentService) : ControllerBase
{
    private int GetUserId()
    => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
   

    [HttpPost]
    public async Task<IActionResult> CreatePayment(
        [FromBody] CreatePaymentRequest request,
        CancellationToken cancellationToken)
    {
        var result = await paymentService.CreatePaymentAsync(
            GetUserId(),
            request,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error.Description);

        return Ok(result.Value);
    }

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetPaymentByOrder(
        int orderId,
        CancellationToken cancellationToken)
    {
        var result = await paymentService.GetPaymentByOrderIdAsync(
            GetUserId(),
            orderId,
            cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error.Description);

        return Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPayments(CancellationToken cancellationToken)
    {
        var result = await paymentService.GetMyPaymentsAsync(
            GetUserId(),
            cancellationToken);

        return Ok(result.Value);
    }

    [HttpPost("stripe/create-intent")]
    public async Task<IActionResult> CreateStripeIntent(
    [FromBody] CreateStripeIntentRequest request,
    CancellationToken cancellationToken)
    {
        var result = await paymentService.CreateStripeIntentAsync(
            GetUserId(),
            request.OrderId,
            cancellationToken);

        if (!result.IsSuccess)
            return BadRequest(result.Error.Description);

        return Ok(result.Value);
    }
}