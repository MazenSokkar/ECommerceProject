using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Banners;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/banners")]
public class BannersController(IBannerService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetActiveBanners(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(includeInactive: false, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Banners fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("admin")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetAllAdmin(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(includeInactive: true, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "All banners fetched successfully."))
            : result.ToProblem();
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Banner fetched successfully."))
            : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> Create([FromBody] CreateBannerRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, ResponseGenerator.GenerateSuccessResponse(result.Value, "Banner created successfully."))
            : result.ToProblem();
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBannerRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Banner updated successfully."))
            : result.ToProblem();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = DefaultRoles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Banner deleted successfully."))
            : result.ToProblem();
    }
}
