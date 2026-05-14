using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.Users;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class UserProfileController(IUserProfileService service) : ControllerBase
{
    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await service.GetAsync(GetCurrentUserId(), cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Profile fetched successfully."))
            : result.ToProblem();
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(GetCurrentUserId(), request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Profile updated successfully."))
            : result.ToProblem();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(GetCurrentUserId(), cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("Account deleted successfully."))
            : result.ToProblem();
    }
}