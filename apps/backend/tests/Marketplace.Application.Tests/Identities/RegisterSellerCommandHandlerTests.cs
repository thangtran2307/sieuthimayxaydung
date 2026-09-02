using Marketplace.Application.Identities;
using Marketplace.Application.Tests.TestDoubles;
using Marketplace.Domain.Common;
using Marketplace.Domain.Identities;

namespace Marketplace.Application.Tests.Identities;

public sealed class RegisterSellerCommandHandlerTests
{
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakePasswordHasher _hasher = new();
    private readonly FakeJwtTokenService _jwt = new();
    private readonly StubClock _clock = new(new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

    private RegisterSellerCommandHandler Handler => new(_unitOfWork, _hasher, _jwt, _clock);

    private static RegisterSellerCommand Command(string email = "New@Example.com") =>
        new(new RegisterSellerRequest(email, "supersecret", "Seller Co.", null, "Hà Nội"));

    [Fact]
    public async Task Registers_a_seller_with_a_hashed_password_and_issues_tokens()
    {
        _unitOfWork.Users.ToReturn = null; // no existing account

        var result = await Handler.Handle(Command(), CancellationToken.None);

        var saved = Assert.Single(_unitOfWork.Users.Added);
        Assert.Equal("new@example.com", saved.Email); // normalized to lowercase
        Assert.Equal("hashed:supersecret", saved.PasswordHash);
        Assert.Equal(UserRole.SELLER, saved.Role);
        Assert.Equal(1, _unitOfWork.SaveChangesCallCount);

        Assert.Equal(_jwt.TokenToIssue, result.Tokens);
        Assert.Equal(saved.Id, _jwt.IssuedFor.UserId);
        Assert.Equal("new@example.com", result.User.Email);
    }

    [Fact]
    public async Task Rejects_a_duplicate_email()
    {
        _unitOfWork.Users.ToReturn = User.Register("new@example.com", "hash", "Existing", _clock.UtcNow);

        var ex = await Assert.ThrowsAsync<ConflictException>(
            () => Handler.Handle(Command(), CancellationToken.None).AsTask());
        Assert.Equal(ErrorCodes.EmailInUse, ex.Code);
        Assert.Empty(_unitOfWork.Users.Added);
    }
}
