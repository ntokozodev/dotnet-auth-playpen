using AuthPlaypen.Application.Dtos;

namespace AuthPlaypen.Application.Services;

public sealed class NoOpOpenIddictScopeSyncService : IOpenIddictScopeSyncService
{
    public Task HandleScopeCreationAsync(ScopeDto dto, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task HandleScopeUpdateAsync(ScopeDto dto, CancellationToken cancellationToken) => Task.CompletedTask;

    public Task HandleScopeDeletionAsync(Guid scopeId, CancellationToken cancellationToken) => Task.CompletedTask;
}

