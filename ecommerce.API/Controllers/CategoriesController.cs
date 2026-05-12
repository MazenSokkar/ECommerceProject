using ecommerce.Contracts.Abstractions.Constants;
using ecommerce.Contracts.Categories;
using ecommerce.Core.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/categories")]

    public class CategoriesController(ICategoryService service) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await service.GetAllAsync(cancellationToken);
            return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Categories fetched successfully."));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var result = await service.GetByIdAsync(id, cancellationToken);
            return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Category fetched successfully."));
        }
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await service.CreateAsync(request, cancellationToken);
            return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Category created successfully."));

        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await service.UpdateAsync(id, request, cancellationToken);
            return Ok(ResponseGenerator.GenerateSuccessResponse(result.Value, "Category updated  successfully."));

        }
        [HttpDelete("{id}")]
        [Authorize(Roles = DefaultRoles.Admin)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await service.DeleteAsync(id, cancellationToken);

            return Ok(ResponseGenerator.GenerateSuccessResponse("Category Deleted  successfully."));

        }

    }
}
