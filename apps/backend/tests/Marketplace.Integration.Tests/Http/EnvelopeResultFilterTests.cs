using Marketplace.Api.Http;
using Marketplace.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace Marketplace.Integration.Tests.Http;

/// <summary>
/// Unit tests for the success-envelope filter (no database needed). Regression cover for the
/// paged-result serialization bug: the filter must clear <see cref="ObjectResult.DeclaredType"/>
/// after swapping the value, otherwise the output formatter casts the envelope to the action's
/// declared type and throws.
/// </summary>
public sealed class EnvelopeResultFilterTests
{
    [Fact]
    public async Task Wraps_paged_result_as_data_and_meta_and_clears_declared_type()
    {
        var page = PagedResult<string>.Create(["a", "b"], total: 2, page: 1, pageSize: 20);
        var objectResult = new ObjectResult(page) { DeclaredType = typeof(PagedResult<string>) };

        var context = await RunFilterAsync(objectResult);

        var wrapped = ((ObjectResult)context.Result).Value;
        Assert.Null(((ObjectResult)context.Result).DeclaredType);

        var data = wrapped!.GetType().GetProperty("data")!.GetValue(wrapped);
        var meta = wrapped.GetType().GetProperty("meta")!.GetValue(wrapped);
        Assert.NotNull(data);
        Assert.IsType<PageMeta>(meta);
    }

    [Fact]
    public async Task Wraps_single_object_as_data_and_clears_declared_type()
    {
        var objectResult = new ObjectResult("hello") { DeclaredType = typeof(string) };

        var context = await RunFilterAsync(objectResult);

        var wrapped = ((ObjectResult)context.Result).Value;
        Assert.Null(((ObjectResult)context.Result).DeclaredType);
        Assert.Equal("hello", wrapped!.GetType().GetProperty("data")!.GetValue(wrapped));
    }

    private static async Task<ResultExecutingContext> RunFilterAsync(ObjectResult objectResult)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());
        var executing = new ResultExecutingContext(
            actionContext,
            [],
            objectResult,
            controller: new object());

        await new EnvelopeResultFilter().OnResultExecutionAsync(
            executing,
            () => Task.FromResult(new ResultExecutedContext(actionContext, [], objectResult, new object())));

        return executing;
    }
}
