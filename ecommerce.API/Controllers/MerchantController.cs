using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Sellers;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ecommerce.Contracts.Abstractions;
namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/Merchants")]
public class SellersController(IMerchantService service) : ControllerBase
{
    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Sellers fetched successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Seller fetched successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpPost("register")]
    [Authorize]
    public async Task<IActionResult> Register([FromBody] CreateMerchantRequest request, CancellationToken cancellationToken)
    {
        var result = await service.RegisterAsync(GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Seller registered successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpPut("me")]
    [Authorize(Roles = DefaultRoles.Merchant)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateMerchantRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateProfileAsync(GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Profile updated successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateMerchantStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateStatusAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Seller status updated successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }
}