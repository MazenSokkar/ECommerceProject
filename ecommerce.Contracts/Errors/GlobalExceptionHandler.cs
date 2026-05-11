using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ecommerce.Contracts.Errors;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception
            , CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Error Occured: [Message]", exception.Message);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal Server Error",
            Type = "https://datatracker.ietf.org/doc/rfc500/"
        };

        httpContext.Response.StatusCode = StatusCodes
            .Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails,
            cancellationToken: cancellationToken);

        return true;
    }
}