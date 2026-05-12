using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Reviews;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/reviews")]
public class ReviewsController(IReviewService service) : ControllerBase
{
    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId, CancellationToken cancellationToken)
    {
        var result = await service.GetByProductIdAsync(productId, cancellationToken);
        return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Reviews fetched successfully."));
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Customer)]
    public async Task<IActionResult> Create([FromBody] CreateReviewRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Review created successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(GetCurrentUserId(), id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Review deleted successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }
}