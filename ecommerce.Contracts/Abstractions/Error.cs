using System;

namespace ecommerce.Contracts.Abstractions;

public record Error(string Code, string Description, int? statusCode)
{
    public static readonly Error None = new(string.Empty, string.Empty, null);
}
