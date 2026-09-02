using Marketplace.Application.Common.Auth;
using Marketplace.Application.Identities;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Identities;

namespace Marketplace.Application.Tests.Identities;

public sealed class RefreshTokenCommandHandlerTests
{
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeJwtTokenService _jwt = new();

    private RefreshTokenCommandHandler Handler => new(_unitOfWork, _jwt);

    [Fact]
    public async Task Rotates_tokens_for_a_valid_refresh_token()
    {
        var user = User.Register("seller@example.com", "hash", "Seller Co.", DateTime.UtcNow);
        _unitOfWork.Users.ToReturn = user;
        _jwt.RefreshResult = new AuthPrincipal(user.Id, UserRole.SELLER, user.Email);

        var result = await Handler.Handle(new RefreshTokenCommand("refresh"), CancellationToken.None);

        Assert.Equal(_jwt.TokenToIssue, result.Tokens);
        Assert.Equal(user.Id, _jwt.IssuedFor.UserId);
    }

    [Fact]
    public async Task Rejects_an_invalid_refresh_token()
    {
        _jwt.RefreshResult = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new RefreshTokenCommand("bad"), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Rejects_a_token_for_a_deleted_account()
    {
        _jwt.RefreshResult = new AuthPrincipal(Guid.NewGuid(), UserRole.SELLER, "seller@example.com");
        _unitOfWork.Users.ToReturn = null; // account no longer exists

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new RefreshTokenCommand("refresh"), CancellationToken.None).AsTask());
    }
}
