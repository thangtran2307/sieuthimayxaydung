using Asp.Versioning;
using Marketplace.Application.Samples;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Marketplace.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("samples")]
public class SamplesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetSamples(CancellationToken cancellationToken)
    {
        var samples = await mediator.Send(new GetAllSampleQuery(), cancellationToken);
        return Ok(samples);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSample(Guid id, CancellationToken cancellationToken)
    {
        var sample = await mediator.Send(new GetSampleByIdQuery { Id = id }, cancellationToken)
            ?? throw new NotFoundException($"Sample with ID '{id}' not found.");

        return Ok(sample);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSample([FromBody] CreateSampleRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateSampleCommand(request);
        var sample = await mediator.Send(command, cancellationToken);
        return Ok(sample);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateSample([FromRoute] Guid id, [FromBody] CreateSampleRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateSampleCommand(id, request);
        var sample = await mediator.Send(command, cancellationToken);
        return Ok(sample);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteSample([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteSampleCommand(id);
        var sample = await mediator.Send(command, cancellationToken);
        return Ok(sample);
    }
}
