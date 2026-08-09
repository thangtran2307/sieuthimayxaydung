namespace Marketplace.Domain.Common;

/// <summary>
/// Base application/domain error carrying a stable code. The API layer maps concrete subtypes to
/// HTTP status codes and the standard error envelope (Constitution I &amp; III: consistent, loud).
/// </summary>
public abstract class AppException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

/// <summary>A domain invariant/rule was violated (maps to 400/422 depending on subtype).</summary>
public class DomainRuleException(string message, string code = ErrorCodes.ValidationFailed)
    : AppException(code, message);

public sealed class NotFoundException(string message = "Resource not found", string code = ErrorCodes.NotFound)
    : AppException(code, message);

public sealed class ForbiddenException(string message = "Forbidden", string code = ErrorCodes.Forbidden)
    : AppException(code, message);

public sealed class UnauthenticatedException(string message = "Not authenticated")
    : AppException(ErrorCodes.Unauthenticated, message);

public sealed class ConflictException(string code, string message = "Conflict")
    : AppException(code, message);

/// <summary>Boundary validation failed; carries per-field details for the envelope.</summary>
public sealed class ValidationAppException(IReadOnlyList<ValidationError> errors, string message = "Validation failed")
    : AppException(ErrorCodes.ValidationFailed, message)
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors;
}
