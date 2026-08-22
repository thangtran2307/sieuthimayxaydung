using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Samples;

public sealed record DeleteSampleCommand(Guid Id) : ICommand<SampleDto>;

public sealed class DeleteSampleCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<DeleteSampleCommand, SampleDto>
{
    public async ValueTask<SampleDto> Handle(DeleteSampleCommand command, CancellationToken cancellationToken)
    {
        var sample = await unitOfWork.SampleRepository.GetByIdAsync(command.Id, cancellationToken) ?? throw new NotFoundException($"Sample with ID {command.Id} not found.");
        unitOfWork.SampleRepository.Remove(sample);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SampleDto(sample.Id, sample.FieldOne, sample.FieldTwo);
    }
}
