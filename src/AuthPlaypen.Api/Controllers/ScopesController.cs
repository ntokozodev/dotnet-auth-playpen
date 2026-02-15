using AuthPlaypen.Api.Dtos;
using AuthPlaypen.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthPlaypen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScopesController(IScopeService scopeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ScopeDto>>> GetAll(CancellationToken cancellationToken)
    {
        var scopes = await scopeService.GetAllAsync(cancellationToken);
        return Ok(scopes);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ScopeDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var scope = await scopeService.GetByIdAsync(id, cancellationToken);
        return scope is null ? NotFound() : Ok(scope);
    }

    [HttpPost]
    public async Task<ActionResult<ScopeDto>> Create(CreateScopeRequest request, CancellationToken cancellationToken)
    {
        var (scope, error) = await scopeService.CreateAsync(request, cancellationToken);
        if (error is not null)
        {
            return BadRequest(error);
        }

        return CreatedAtAction(nameof(GetById), new { id = scope!.Id }, scope);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ScopeDto>> Update(Guid id, UpdateScopeRequest request, CancellationToken cancellationToken)
    {
        var (scope, error, notFound) = await scopeService.UpdateAsync(id, request, cancellationToken);
        if (notFound)
        {
            return NotFound();
        }

        if (error is not null)
        {
            return BadRequest(error);
        }

        return Ok(scope);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var (deleted, error, notFound) = await scopeService.DeleteAsync(id, cancellationToken);
        if (notFound)
        {
            return NotFound();
        }

        if (error is not null)
        {
            return BadRequest(error);
        }

        return deleted ? NoContent() : NotFound();
    }
}
