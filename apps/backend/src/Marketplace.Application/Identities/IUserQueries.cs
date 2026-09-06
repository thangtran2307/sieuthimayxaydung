namespace Marketplace.Application.Identities;

/// <summary>Read side (Dapper) for user accounts. Returns wire DTOs; never credentials.</summary>
public interface IUserQueries
{
    /// <summary>Returns the account summary for a user id, or null when it does not exist.</summary>
    Task<AuthUserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
