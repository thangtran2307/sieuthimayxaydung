using FluentValidation;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Identities;

/// <summary>Email/password sign-in payload.</summary>
public sealed record LoginRequest(string Email, string Password);

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

/// <summary>Authenticates a user and issues a session (FR-012).</summary>
public sealed record LoginCommand(LoginRequest Body) : ICommand<AuthResultDto>;

public sealed class LoginCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService) : ICommandHandler<LoginCommand, AuthResultDto>
{
    public async ValueTask<AuthResultDto> Handle(LoginCommand command, CancellationToken cancellationToken)
    {
        string email = command.Body.Email.Trim().ToLowerInvariant();

        var user = await unitOfWork.UserRepository.FirstOrDefaultAsync(
            u => u.Email == email,
            cancellationToken);

        // Same error whether the email is unknown or the password is wrong (no account enumeration).
        if (user is null
            || string.IsNullOrEmpty(user.PasswordHash)
            || !passwordHasher.Verify(user.PasswordHash, command.Body.Password))
        {
            throw new UnauthenticatedException("Invalid email or password.", ErrorCodes.InvalidCredentials);
        }

        var tokens = jwtTokenService.Issue(new AuthPrincipal(user.Id, user.Role, user.Email));
        return new AuthResultDto(tokens, new AuthUserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }
}
