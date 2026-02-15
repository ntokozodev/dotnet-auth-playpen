using AuthPlaypen.Api.Data;
using AuthPlaypen.Api.Dtos;
using AuthPlaypen.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthPlaypen.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController(AuthPlaypenDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ApplicationDto>>> GetAll(CancellationToken cancellationToken)
    {
        var applications = await dbContext.Applications
            .Include(a => a.ApplicationScopes)
            .ThenInclude(a => a.Scope)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var globalScopes = await dbContext.Scopes
            .Where(s => s.IsGlobal)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Ok(applications.Select(a => ToDto(a, globalScopes)).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ApplicationDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications
            .Include(a => a.ApplicationScopes)
            .ThenInclude(a => a.Scope)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (application is null)
        {
            return NotFound();
        }

        var globalScopes = await dbContext.Scopes
            .Where(s => s.IsGlobal)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return Ok(ToDto(application, globalScopes));
    }

    [HttpPost]
    public async Task<ActionResult<ApplicationDto>> Create(CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        if (request.ScopeIds is null || request.ScopeIds.Count == 0)
        {
            return BadRequest("Application must include at least one scope.");
        }

        var redirectUrisValidationError = ValidateRedirectUris(request.Flow, request.RedirectUris, request.PostLogoutRedirectUris);
        if (redirectUrisValidationError is not null)
        {
            return BadRequest(redirectUrisValidationError);
        }

        var scopes = await dbContext.Scopes
            .Where(s => request.ScopeIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (scopes.Count != request.ScopeIds.Distinct().Count())
        {
            return BadRequest("One or more scope IDs are invalid.");
        }

        var application = new ApplicationEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            ClientId = request.ClientId,
            ClientSecret = request.ClientSecret,
            Flow = request.Flow,
            PostLogoutRedirectUris = request.PostLogoutRedirectUris,
            RedirectUris = request.RedirectUris
        };

        foreach (var scope in scopes.Where(s => !s.IsGlobal))
        {
            application.ApplicationScopes.Add(new ApplicationScopeEntity
            {
                ApplicationId = application.Id,
                ScopeId = scope.Id
            });
        }

        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = application.Id }, ToDto(application, scopes.Where(s => s.IsGlobal).ToList()));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApplicationDto>> Update(Guid id, UpdateApplicationRequest request, CancellationToken cancellationToken)
    {
        if (request.ScopeIds is null || request.ScopeIds.Count == 0)
        {
            return BadRequest("Application must include at least one scope.");
        }

        var redirectUrisValidationError = ValidateRedirectUris(request.Flow, request.RedirectUris, request.PostLogoutRedirectUris);
        if (redirectUrisValidationError is not null)
        {
            return BadRequest(redirectUrisValidationError);
        }

        var application = await dbContext.Applications
            .Include(a => a.ApplicationScopes)
            .ThenInclude(a => a.Scope)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (application is null)
        {
            return NotFound();
        }

        var scopes = await dbContext.Scopes
            .Where(s => request.ScopeIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (scopes.Count != request.ScopeIds.Distinct().Count())
        {
            return BadRequest("One or more scope IDs are invalid.");
        }

        application.DisplayName = request.DisplayName;
        application.ClientId = request.ClientId;
        application.ClientSecret = request.ClientSecret;
        application.Flow = request.Flow;
        application.PostLogoutRedirectUris = request.PostLogoutRedirectUris;
        application.RedirectUris = request.RedirectUris;

        application.ApplicationScopes.Clear();
        foreach (var scope in scopes.Where(s => !s.IsGlobal))
        {
            application.ApplicationScopes.Add(new ApplicationScopeEntity
            {
                ApplicationId = application.Id,
                ScopeId = scope.Id
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(application, scopes.Where(s => s.IsGlobal).ToList()));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (application is null)
        {
            return NotFound();
        }

        dbContext.Applications.Remove(application);
        await dbContext.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static ApplicationDto ToDto(ApplicationEntity application, IReadOnlyCollection<ScopeEntity> globalScopes)
    {
        var scopeDtos = globalScopes
            .Concat(application.ApplicationScopes.Select(x => x.Scope))
            .GroupBy(s => s.Id)
            .Select(g => g.First())
            .Select(s => new ScopeReferenceDto(s.Id, s.DisplayName, s.ScopeName, s.Description))
            .ToList();

        return new ApplicationDto(
            application.Id,
            application.DisplayName,
            application.ClientId,
            application.ClientSecret,
            application.Flow,
            application.PostLogoutRedirectUris,
            application.RedirectUris,
            scopeDtos);
    }

    private static string? ValidateRedirectUris(ApplicationFlow flow, string? redirectUris, string? postLogoutRedirectUris)
    {
        if (flow == ApplicationFlow.AuthorizationWithPKCE)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(redirectUris) || !string.IsNullOrWhiteSpace(postLogoutRedirectUris))
        {
            return "RedirectUris and PostLogoutRedirectUris are only allowed for AuthorizationWithPKCE flow.";
        }

        return null;
    }
}
