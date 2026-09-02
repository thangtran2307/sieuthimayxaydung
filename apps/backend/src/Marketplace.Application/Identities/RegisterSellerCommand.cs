using FluentValidation;
using Marketplace.Application.Common.Auth;
using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Identities;
using Mediator;

namespace Marketplace.Application.Identities;

/// <summary>Seller self-registration payload (email/password).</summary>
public sealed record RegisterSellerRequest(
    string Email,
    string Password,
    string DisplayName,
    string Phone,
    string LocationProvince);

public sealed class RegisterSellerRequestValidator : AbstractValidator<RegisterSellerRequest>
{
    public RegisterSellerRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(255);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128);
        RuleFor(x => x.DisplayName).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Phone).MaximumLength(32);
        RuleFor(x => x.LocationProvince).MaximumLength(120);
    }
}

/// <summary>Registers a new seller and issues an authenticated session (FR-012, FR-013a).</summary>
public sealed record RegisterSellerCommand(RegisterSellerRequest Body) : ICommand<AuthResultDto>;

public sealed class RegisterSellerCommandHandler(
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    IJwtTokenService jwtTokenService,
    IClock clock) : ICommandHandler<RegisterSellerCommand, AuthResultDto>
{
    public async ValueTask<AuthResultDto> Handle(
        RegisterSellerCommand command,
        CancellationToken cancellationToken)
    {
        var body = command.Body;
        string email = body.Email.Trim().ToLowerInvariant();

        var existing = await unitOfWork.UserRepository.FirstOrDefaultAsync(
            u => u.Email == email,
            cancellationToken);
        if (existing is not null)
        {
            throw new ConflictException(ErrorCodes.EmailInUse, "An account with this email already exists.");
        }

        var user = User.Register(
            email,
            passwordHasher.Hash(body.Password),
            body.DisplayName.Trim(),
            clock.UtcNow,
            string.IsNullOrWhiteSpace(body.Phone) ? null : body.Phone.Trim(),
            string.IsNullOrWhiteSpace(body.LocationProvince) ? null : body.LocationProvince.Trim());

        await unitOfWork.UserRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var tokens = jwtTokenService.Issue(new AuthPrincipal(user.Id, user.Role, user.Email));
        return new AuthResultDto(tokens, new AuthUserDto(user.Id, user.Email, user.DisplayName, user.Role));
    }
}
