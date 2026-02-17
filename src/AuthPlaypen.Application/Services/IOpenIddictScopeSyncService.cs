using AuthPlaypen.Application.Dtos;

namespace AuthPlaypen.Application.Services;

public interface IOpenIddictScopeSyncService
{
    Task HandleScopeCreationAsync(ScopeDto dto, CancellationToken cancellationToken);
    Task HandleScopeUpdateAsync(ScopeDto dto, CancellationToken cancellationToken);
    Task HandleScopeDeletionAsync(Guid scopeId, CancellationToken cancellationToken);
}

