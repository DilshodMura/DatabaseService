using DbService.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class SynchronizationController : ControllerBase
{
    private readonly ISynchronizationService _synchronizationService;

    public SynchronizationController(ISynchronizationService synchronizationService)
    {
        _synchronizationService = synchronizationService;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(CancellationToken cancellationToken)
    {
        await _synchronizationService.StartAsync(cancellationToken);
        return Ok();
    }

    [HttpPost("stop")]
    public async Task<IActionResult> Stop(CancellationToken cancellationToken)
    {
        await _synchronizationService.StopAsync(cancellationToken);
        return Ok();
    }
}