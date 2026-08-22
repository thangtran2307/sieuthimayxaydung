using FluentValidation;

namespace Marketplace.Application.Samples;

public sealed record CreateSampleRequest
{
    public string FieldOne { get; init; }
    public string FieldTwo { get; init; }
}

public sealed class CreateSampleRequestValidator : AbstractValidator<CreateSampleRequest>
{
    public CreateSampleRequestValidator()
    {
        RuleFor(x => x.FieldOne).NotEmpty().MaximumLength(255);
        RuleFor(x => x.FieldTwo).NotEmpty().MaximumLength(255);
    }
}
