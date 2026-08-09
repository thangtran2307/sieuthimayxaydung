using Marketplace.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Marketplace.Api.Http;

/// <summary>
/// Wraps successful controller results in the standard success envelope (Constitution I):
/// paged results become `{ data, meta }`; everything else becomes `{ data }`.
/// Error responses (status >= 400) and ProblemDetails are left untouched.
/// </summary>
public sealed class EnvelopeResultFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult { Value: not null } result
            && result.Value is not ProblemDetails
            && result.StatusCode is null or < 400)
        {
            result.Value = result.Value is IPagedResult paged
                ? new { data = paged.Items, meta = paged.Meta }
                : new { data = result.Value };
        }

        await next();
    }
}
