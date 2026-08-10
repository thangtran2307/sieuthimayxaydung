namespace Marketplace.Api.Middleware;

/// <summary>
/// Translates every unhandled exception into the standard error envelope
/// `{ error: { code, message, details } }` and logs it (Constitution I &amp; III: consistent, loud).
/// </summary>
public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationAppException ex)
        {
            await WriteAsync(context, StatusCodes.Status422UnprocessableEntity, ex.Code, ex.Message, ex.Errors);
        }
        catch (AppException ex)
        {
            int status = MapStatus(ex);
            if (status >= 500)
            {
                logger.LogError(ex, "Application error: {Code}", ex.Code);
            }

            await WriteAsync(context, status, ex.Code, ex.Message, null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.InternalError,
                "Something went wrong",
                null);
        }
    }

    private static int MapStatus(AppException ex) => ex switch
    {
        NotFoundException => StatusCodes.Status404NotFound,
        ForbiddenException => StatusCodes.Status403Forbidden,
        UnauthenticatedException => StatusCodes.Status401Unauthorized,
        ConflictException => StatusCodes.Status409Conflict,
        DomainRuleException => StatusCodes.Status422UnprocessableEntity,
        _ => StatusCodes.Status400BadRequest,
    };

    private static async Task WriteAsync(
        HttpContext context,
        int status,
        string code,
        string message,
        object details)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(
            new { error = new { code, message, details } },
            CancellationToken.None);
    }
}
