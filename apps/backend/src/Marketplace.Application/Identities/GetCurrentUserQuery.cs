using Marketplace.Application.Common.Auth;
using Mediator;

namespace Marketplace.Application.Identities;

/// <summary>Returns the authenticated caller's account summary (the /auth/me endpoint).</summary>
public sealed record GetCurrentUserQuery : IQuery<AuthUserDto>;

public sealed class GetCurrentUserQueryHandler(
    ICurrentUser currentUser,
    IUserQueries userQueries) : IQueryHandler<GetCurrentUserQuery, AuthUserDto>
{
    public async ValueTask<AuthUserDto> Handle(GetCurrentUserQuery query, CancellationToken cancellationToken)
    {
        var userId = currentUser.RequireUserId();

        // Token is valid but the account is gone → treat as an ended session.
        return await userQueries.GetByIdAsync(userId, cancellationToken)
            ?? throw new UnauthenticatedException();
    }
}
