using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace ecommerce.Contracts.Abstractions;

public static class ResultExtensions
{
    public static ObjectResult ToProblem(this Result result, int statusCode)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Can't convert success result to problem");


        var problem = Results.Problem(statusCode:
            result.Error.statusCode);

        var problemDetails = problem.GetType()
            .GetProperty(nameof(ProblemDetails))
                !.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
        {
            {
                "errors", new[]
                    {
                        result.Error.Code,
                        result.Error.Description
                    }
            }
        };

        return new ObjectResult(problemDetails);
    }
    public static ObjectResult ToProblem<T>(this Result<T> result, int statusCode)
    {
        if (result.IsSuccess)
            throw new InvalidOperationException("Can't convert success result to problem");

        var problem = Results.Problem(statusCode: result.Error.statusCode);

        var problemDetails = problem.GetType()
            .GetProperty(nameof(ProblemDetails))
                !.GetValue(problem) as ProblemDetails;

        problemDetails!.Extensions = new Dictionary<string, object?>
    {
        {
            "errors", new[]
                {
                    result.Error.Code,
                    result.Error.Description
                }
        }
    };

        return new ObjectResult(problemDetails);
    }
}