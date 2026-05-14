using System;
using ecommerce.Contracts.Abstractions;

namespace ecommerce.API.Controllers;

public static class ResponseGenerator
{
    public static APIResponse<T> GenerateSuccessResponse<T>(T? data, string message) where T : class
    {
        return new APIResponse<T>
        {
            Data = data,
            Message = message,
            IsSuccess = true
        };
    }

    public static APIResponse<object> GenerateSuccessResponse(string message)
    {
        return new APIResponse<object>
        {
            Data = null,
            Message = message,
            IsSuccess = true
        };
    }
}
