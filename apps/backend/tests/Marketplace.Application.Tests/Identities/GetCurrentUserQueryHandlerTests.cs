using Marketplace.Application.Identities;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;

namespace Marketplace.Application.Tests.Identities;

public sealed class GetCurrentUserQueryHandlerTests
{
    private readonly FakeCurrentUser _currentUser = new() { UserId = Guid.NewGuid() };
    private readonly FakeUserQueries _userQueries = new();

    private GetCurrentUserQueryHandler Handler => new(_currentUser, _userQueries);

    [Fact]
    public async Task Returns_the_authenticated_account()
    {
        _userQueries.Result = new AuthUserDto(
            _currentUser.UserId!.Value,
            "seller@example.com",
            "Seller Co.",
            UserRole.SELLER);

        var result = await Handler.Handle(new GetCurrentUserQuery(), CancellationToken.None);

        Assert.Equal("seller@example.com", result.Email);
        Assert.Equal("Seller Co.", result.DisplayName);
    }

    [Fact]
    public async Task Throws_when_the_account_no_longer_exists()
    {
        _userQueries.Result = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new GetCurrentUserQuery(), CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task Throws_when_the_caller_is_anonymous()
    {
        _currentUser.UserId = null;

        await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(new GetCurrentUserQuery(), CancellationToken.None).AsTask());
    }
}
