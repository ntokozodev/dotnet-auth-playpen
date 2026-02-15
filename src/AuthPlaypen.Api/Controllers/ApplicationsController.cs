using AuthPlaypen.Api.Dtos;
using AuthPlaypen.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlaypen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController(IApplicationService applicationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ApplicationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var applications = await applicationService.GetAllAsync(cancellationToken);
        return Ok(applications);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApplicationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var application = await applicationService.GetByIdAsync(id, cancellationToken);
        return application is null ? NotFound() : Ok(application);
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> Create(CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        var (application, error) = await applicationService.CreateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(error);
        }

        return CreatedAtAction(nameof(GetById), new { id = application!.Id }, application);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApplicationDto>> Update(Guid id, UpdateApplicationRequest request, CancellationToken cancellationToken)
    {
        var (application, error, notFound) = await applicationService.UpdateAsync(id, request, cancellationToken);
        if (notFound)
        {
            return NotFound();
        }

        if (error is not null)
        {
            return BadRequest(error);
        }

        return Ok(application);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await applicationService.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
