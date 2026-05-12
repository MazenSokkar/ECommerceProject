using Microsoft.AspNetCore.Http;
using ecommerce.Contracts.Abstractions;

namespace ecommerce.Contracts.Errors;

public static class CategoryErrors
{
    public static readonly Error NotFound =
        new("Category.NotFound", "Category not found.", StatusCodes.Status404NotFound);

    public static readonly Error DuplicateName =
        new("Category.DuplicateName", "A category with this name already exists.", StatusCodes.Status409Conflict);

    public static readonly Error HasProducts =
        new("Category.HasProducts", "Cannot delete a category that has products.", StatusCodes.Status409Conflict);
}