namespace Marketplace.Api.Middleware;

/// <summary>
/// Translates every unhandled exception into the standard error envelope
/// `{ error: { code, message, details } }` and logs it before responding (Constitution I &amp; III:
/// consistent, loud) — 4xx client errors at Warning, 5xx server faults at Error.
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
            Log(context, ex, StatusCodes.Status422UnprocessableEntity, ex.Code);
            await WriteAsync(context, StatusCodes.Status422UnprocessableEntity, ex.Code, ex.Message, ex.Errors);
        }
        catch (AppException ex)
        {
            int status = MapStatus(ex);
            Log(context, ex, status, ex.Code);
            await WriteAsync(context, status, ex.Code, ex.Message, null);
        }
        catch (Exception ex)
        {
            Log(context, ex, StatusCodes.Status500InternalServerError, ErrorCodes.InternalError);
            await WriteAsync(
                context,
                StatusCodes.Status500InternalServerError,
                ErrorCodes.InternalError,
                "Something went wrong",
                null);
        }
    }

    // Every exception is logged before the response is written so it can be reviewed later. Expected
    // client errors (4xx) log at Warning; server faults (5xx) log at Error. The exception (with stack)
    // is always attached.
    private void Log(HttpContext context, Exception ex, int status, string code)
    {
        var level = status >= StatusCodes.Status500InternalServerError ? LogLevel.Error : LogLevel.Warning;

        // Guard so the message arguments are only evaluated when this level is enabled (Sonar S6664).
        if (!logger.IsEnabled(level))
        {
            return;
        }

        logger.Log(
            level,
            ex,
            "Request {Method} {Path} failed with {Status} {Code}",
            context.Request.Method,
            context.Request.Path,
            status,
            code);
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
