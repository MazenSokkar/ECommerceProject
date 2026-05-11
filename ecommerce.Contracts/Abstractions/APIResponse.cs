using System;

namespace ecommerce.Contracts.Abstractions;

public class APIResponse<T> where T : class
{
    public T? Data { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
}
