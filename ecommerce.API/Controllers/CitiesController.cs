using ecommerce.Contracts.Abstractions;
using ecommerce.Contracts.City;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers;

[ApiController]
[Route("api/cities")]
public class CitiesController(ICityService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await service.GetAllAsync(cancellationToken);
        return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Cities fetched successfully."));
    }

    [HttpGet("by-state-province/{stateProvinceId}")]
    public async Task<IActionResult> GetByStateProvince(int stateProvinceId, CancellationToken cancellationToken)
    {
        var result = await service.GetByStateProvinceAsync(stateProvinceId, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Cities fetched successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "City fetched successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCityRequest request, CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "City created successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCityRequest request, CancellationToken cancellationToken)
    {
        var result = await service.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "City updated successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await service.DeleteAsync(id, cancellationToken);
        return result.IsSuccess
            ? Ok(ResponseGenerator.GenerateSuccessResponse("City deleted successfully."))
            : result.ToProblem(result.Error.statusCode ?? StatusCodes.Status400BadRequest);
    }
}
