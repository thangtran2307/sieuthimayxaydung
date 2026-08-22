using Marketplace.Application.Common.Persistence;
using Marketplace.Domain.Samples;
using Mediator;

namespace Marketplace.Application.Samples;

public sealed record CreateSampleCommand(CreateSampleRequest Request) : ICommand<SampleDto>;

public sealed class CreateSampleCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<CreateSampleCommand, SampleDto>
{
    public async ValueTask<SampleDto> Handle(CreateSampleCommand command, CancellationToken cancellationToken)
    {
        var sample = new Sample(command.Request.FieldOne, command.Request.FieldTwo);

        await unitOfWork.SampleRepository.AddAsync(sample, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SampleDto(sample.Id, sample.FieldOne, sample.FieldTwo);
    }
}
