using Marketplace.Application.Common.Persistence;
using Mediator;

namespace Marketplace.Application.Samples;

public sealed record UpdateSampleCommand(Guid Id, CreateSampleRequest Request) : ICommand<SampleDto>;

public sealed class UpdateSampleCommandHandler(IUnitOfWork unitOfWork) : ICommandHandler<UpdateSampleCommand, SampleDto>
{
    public async ValueTask<SampleDto> Handle(UpdateSampleCommand command, CancellationToken cancellationToken)
    {
        var sample = await unitOfWork.SampleRepository.GetByIdAsync(command.Id, cancellationToken) ?? throw new NotFoundException($"Sample with ID {command.Id} not found.");
        sample.Update(command.Request.FieldOne, command.Request.FieldTwo);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new SampleDto(sample.Id, sample.FieldOne, sample.FieldTwo);
    }
}
