using Marketplace.Application.Common.Auth;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Infrastructure.Auth;

/// <summary>Wraps ASP.NET Core's <see cref="PasswordHasher{T}"/> (PBKDF2) — no third-party dep.</summary>
public sealed class PasswordHasherAdapter : IPasswordHasher
{
    private static readonly object Subject = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(Subject, password);

    public bool Verify(string hash, string password) =>
        _hasher.VerifyHashedPassword(Subject, hash, password) != PasswordVerificationResult.Failed;
}
