using Marketplace.Application.Common.Auth;

namespace Marketplace.Application.Identities;

/// <summary>Account information returned to the client after register/login (never credentials).</summary>
public sealed record AuthUserDto(Guid Id, string Email, string DisplayName, UserRole Role);

/// <summary>
/// Result of an authentication use case. The <see cref="Tokens"/> are written to httpOnly cookies by
/// the API layer and never serialized into the response body; only <see cref="User"/> is returned.
/// </summary>
public sealed record AuthResultDto(TokenPair Tokens, AuthUserDto User);
