using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Identities;

/// <summary>
/// Exchanges a valid refresh token for a fresh access + refresh token pair (rotation). The refresh
/// token is read from its httpOnly cookie by the API layer.
/// </summary>
public sealed record RefreshTokenCommand(string RefreshToken) : ICommand<AuthResultDto>;

public sealed class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    IJwtTokenService jwtTokenService) : ICommandHandler<RefreshTokenCommand, AuthResultDto>
{
    public async ValueTask<AuthResultDto> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        var principal = jwtTokenService.ValidateRefreshToken(command.RefreshToken)
            ?? throw new UnauthenticatedException("Invalid or expired session.");

        // Reload the account so a deleted user (or a changed role) cannot keep refreshing.
        var user = await unitOfWork.UserRepository.FirstOrDefaultAsync(
            u => u.Id == principal.UserId,
            cancellationToken)
            ?? throw new UnauthenticatedException("Invalid or expired session.");

        var tokens = jwtTokenService.Issue(new AuthPrincipal(user.Id, user.Role, user.Email));
        return new AuthResultDto(tokens, new AuthUserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }
}
