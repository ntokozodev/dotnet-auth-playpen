using AuthPlaypen.Data.Data;
using AuthPlaypen.Application.Dtos;
using AuthPlaypen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthPlaypen.Application.Services;

public class ApplicationService(AuthPlaypenDbContext dbContext) : IApplicationService
{
    public async Task<IReadOnlyCollection<ApplicationDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var applications = await dbContext.Applications
            .Include(a => a.ApplicationScopes)
            .ThenInclude(a => a.Scope)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return applications.Select(ToDto).ToList();
    }

    public async Task<ApplicationDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications
            .Include(a => a.ApplicationScopes)
            .ThenInclude(a => a.Scope)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (application is null)
        {
            return null;
        }

        return ToDto(application);
    }

    public async Task<(ApplicationDto? Application, string? Error)> CreateAsync(CreateApplicationRequest request, CancellationToken cancellationToken)
    {
        if (request.ScopeIds is null || request.ScopeIds.Count == 0)
        {
            return (null, "Application must include at least one scope.");
        }

        var redirectUrisValidationError = ValidateRedirectUris(request.Flow, request.RedirectUris, request.PostLogoutRedirectUris);
        if (redirectUrisValidationError is not null)
        {
            return (null, redirectUrisValidationError);
        }

        var scopes = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .Where(s => request.ScopeIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (scopes.Count != request.ScopeIds.Distinct().Count())
        {
            return (null, "One or more scope IDs are invalid.");
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

        foreach (var scope in scopes)
        {
            application.ApplicationScopes.Add(new ApplicationScopeEntity
            {
                ApplicationId = application.Id,
                ScopeId = scope.Id
            });
        }

        dbContext.Applications.Add(application);
        await dbContext.SaveChangesAsync(cancellationToken);

        var dto = ToDto(application);
        return (dto, null);
    }

    public async Task<(ApplicationDto? Application, string? Error, bool NotFound)> UpdateAsync(Guid id, UpdateApplicationRequest request, CancellationToken cancellationToken)
    {
        if (request.ScopeIds is null || request.ScopeIds.Count == 0)
        {
            return (null, "Application must include at least one scope.", false);
        }

        var redirectUrisValidationError = ValidateRedirectUris(request.Flow, request.RedirectUris, request.PostLogoutRedirectUris);
        if (redirectUrisValidationError is not null)
        {
            return (null, redirectUrisValidationError, false);
        }

        var application = await dbContext.Applications
            .Include(a => a.ApplicationScopes)
            .ThenInclude(a => a.Scope)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (application is null)
        {
            return (null, null, true);
        }

        var scopes = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .Where(s => request.ScopeIds.Contains(s.Id))
            .ToListAsync(cancellationToken);

        if (scopes.Count != request.ScopeIds.Distinct().Count())
        {
            return (null, "One or more scope IDs are invalid.", false);
        }

        application.DisplayName = request.DisplayName;
        application.ClientId = request.ClientId;
        application.ClientSecret = request.ClientSecret;
        application.Flow = request.Flow;
        application.PostLogoutRedirectUris = request.PostLogoutRedirectUris;
        application.RedirectUris = request.RedirectUris;

        application.ApplicationScopes.Clear();
        foreach (var scope in scopes)
        {
            application.ApplicationScopes.Add(new ApplicationScopeEntity
            {
                ApplicationId = application.Id,
                ScopeId = scope.Id
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        var dto = ToDto(application);
        return (dto, null, false);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var application = await dbContext.Applications.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (application is null)
        {
            return false;
        }

        dbContext.Applications.Remove(application);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ApplicationDto ToDto(ApplicationEntity application)
    {
        var scopeDtos = application.ApplicationScopes
            .Select(x => x.Scope)
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
