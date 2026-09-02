using Marketplace.Application.Identities;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Identities;

namespace Marketplace.Application.Tests.Identities;

public sealed class LoginCommandHandlerTests
{
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeJwtTokenService _jwt = new();

    private LoginCommandHandler Handler => new(_unitOfWork, _hasher, _jwt);

    private User SeededUser(string password = "supersecret") =>
        User.Register("seller@example.com", _hasher.Hash(password), "Seller Co.", DateTime.UtcNow);

    [Fact]
    public async Task Issues_tokens_for_valid_credentials()
    {
        _unitOfWork.Users.ToReturn = SeededUser();

        var result = await Handler.Handle(
            new LoginCommand(new LoginRequest("Seller@Example.com", "supersecret")),
            CancellationToken.None);

        Assert.Equal(_jwt.TokenToIssue, result.Tokens);
        Assert.Equal("seller@example.com", result.User.Email);
    }

    [Fact]
    public async Task Rejects_an_unknown_email()
    {
        _unitOfWork.Users.ToReturn = null;

        var ex = await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(
                new LoginCommand(new LoginRequest("nobody@example.com", "supersecret")),
                CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.InvalidCredentials, ex.Code);
    }

    [Fact]
    public async Task Rejects_a_wrong_password()
    {
        _unitOfWork.Users.ToReturn = SeededUser();

        var ex = await Assert.ThrowsAsync<UnauthenticatedException>(
            () => Handler.Handle(
                new LoginCommand(new LoginRequest("seller@example.com", "wrong-password")),
                CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.InvalidCredentials, ex.Code);
    }
}
