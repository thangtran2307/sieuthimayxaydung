namespace Marketplace.Application.Common.Auth;

/// <summary>Hashes and verifies user passwords (Constitution VI). Implemented in Infrastructure.</summary>
public interface IPasswordHasher
{
    string Hash(string password);

    bool Verify(string hash, string password);
}
