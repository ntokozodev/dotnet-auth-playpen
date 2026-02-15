using AuthPlaypen.Data.Data;
using AuthPlaypen.Application.Dtos;
using AuthPlaypen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthPlaypen.Application.Services;

public class ScopeService(AuthPlaypenDbContext dbContext) : IScopeService
{
    public async Task<IReadOnlyCollection<ScopeDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var scopes = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .ThenInclude(x => x.Application)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return scopes.Select(ToDto).ToList();
    }

    public async Task<ScopeDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var scope = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .ThenInclude(x => x.Application)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        return scope is null ? null : ToDto(scope);
    }

    public async Task<(ScopeDto? Scope, string? Error)> CreateAsync(CreateScopeRequest request, CancellationToken cancellationToken)
    {
        var appIds = request.ApplicationIds?.Distinct().ToList() ?? [];
        if (appIds.Count > 0)
        {
            var appCount = await dbContext.Applications.CountAsync(a => appIds.Contains(a.Id), cancellationToken);
            if (appCount != appIds.Count)
            {
                return (null, "One or more application IDs are invalid.");
            }
        }

        var scope = new ScopeEntity
        {
            Id = Guid.NewGuid(),
            DisplayName = request.DisplayName,
            ScopeName = request.ScopeName,
            Description = request.Description,
            IsGlobal = appIds.Count == 0
        };

        foreach (var appId in appIds)
        {
            scope.ApplicationScopes.Add(new ApplicationScopeEntity
            {
                ApplicationId = appId,
                ScopeId = scope.Id
            });
        }

        dbContext.Scopes.Add(scope);
        await dbContext.SaveChangesAsync(cancellationToken);

        var reloaded = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .ThenInclude(x => x.Application)
            .AsNoTracking()
            .FirstAsync(s => s.Id == scope.Id, cancellationToken);

        return (ToDto(reloaded), null);
    }

    public async Task<(ScopeDto? Scope, string? Error, bool NotFound)> UpdateAsync(Guid id, UpdateScopeRequest request, CancellationToken cancellationToken)
    {
        var scope = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (scope is null)
        {
            return (null, null, true);
        }

        var appIds = request.ApplicationIds?.Distinct().ToHashSet() ?? [];
        if (appIds.Count > 0)
        {
            var appCount = await dbContext.Applications.CountAsync(a => appIds.Contains(a.Id), cancellationToken);
            if (appCount != appIds.Count)
            {
                return (null, "One or more application IDs are invalid.", false);
            }
        }

        var validationError = await ValidateMutationDoesNotBreakMinimumScopeRule(
            scope,
            appIds.Count == 0,
            appIds,
            cancellationToken);

        if (validationError is not null)
        {
            return (null, validationError, false);
        }

        scope.DisplayName = request.DisplayName;
        scope.ScopeName = request.ScopeName;
        scope.Description = request.Description;
        scope.IsGlobal = appIds.Count == 0;

        scope.ApplicationScopes.Clear();
        foreach (var appId in appIds)
        {
            scope.ApplicationScopes.Add(new ApplicationScopeEntity
            {
                ApplicationId = appId,
                ScopeId = scope.Id
            });
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        var reloaded = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .ThenInclude(x => x.Application)
            .AsNoTracking()
            .FirstAsync(s => s.Id == id, cancellationToken);

        return (ToDto(reloaded), null, false);
    }

    public async Task<(bool Deleted, string? Error, bool NotFound)> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var scope = await dbContext.Scopes
            .Include(s => s.ApplicationScopes)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (scope is null)
        {
            return (false, null, true);
        }

        var validationError = await ValidateMutationDoesNotBreakMinimumScopeRule(
            scope,
            false,
            [],
            cancellationToken);

        if (validationError is not null)
        {
            return (false, validationError, false);
        }

        dbContext.Scopes.Remove(scope);
        await dbContext.SaveChangesAsync(cancellationToken);
        return (true, null, false);
    }

    private async Task<string?> ValidateMutationDoesNotBreakMinimumScopeRule(
        ScopeEntity currentScope,
        bool nextIsGlobal,
        IReadOnlySet<Guid> nextAppIds,
        CancellationToken cancellationToken)
    {
        var allApps = await dbContext.Applications
            .AsNoTracking()
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        if (allApps.Count == 0)
        {
            return null;
        }

        var globalCount = await dbContext.Scopes.CountAsync(s => s.IsGlobal, cancellationToken);

        var appSpecificCounts = await dbContext.ApplicationScopes
            .GroupBy(x => x.ApplicationId)
            .Select(g => new { ApplicationId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ApplicationId, x => x.Count, cancellationToken);

        var currentlyScopedAppIds = currentScope.IsGlobal
            ? allApps.ToHashSet()
            : currentScope.ApplicationScopes.Select(x => x.ApplicationId).ToHashSet();

        foreach (var appId in allApps)
        {
            var effectiveCount = globalCount + appSpecificCounts.GetValueOrDefault(appId, 0);

            if (currentScope.IsGlobal)
            {
                effectiveCount -= 1;
            }
            else if (currentlyScopedAppIds.Contains(appId))
            {
                effectiveCount -= 1;
            }

            if (nextIsGlobal || nextAppIds.Contains(appId))
            {
                effectiveCount += 1;
            }

            if (effectiveCount <= 0)
            {
                return "Operation would leave an application without any scope, which is not allowed.";
            }
        }

        return null;
    }

    private static ScopeDto ToDto(ScopeEntity scope)
    {
        var applications = scope.IsGlobal
            ? Array.Empty<ApplicationReferenceDto>()
            : scope.ApplicationScopes
                .Select(x => new ApplicationReferenceDto(
                    x.Application.Id,
                    x.Application.DisplayName,
                    x.Application.ClientId))
                .ToArray();

        return new ScopeDto(scope.Id, scope.DisplayName, scope.ScopeName, scope.Description, applications);
    }
}
