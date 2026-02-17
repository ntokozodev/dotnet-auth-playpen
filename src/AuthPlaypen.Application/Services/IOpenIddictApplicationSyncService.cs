using AuthPlaypen.Application.Dtos;

namespace AuthPlaypen.Application.Services;

public interface IOpenIddictApplicationSyncService
{
    Task HandleApplicationCreationAsync(ApplicationDto dto, CancellationToken cancellationToken);
    Task HandleApplicationUpdateAsync(ApplicationDto dto, CancellationToken cancellationToken);
    Task HandleApplicationDeletionAsync(Guid applicationId, CancellationToken cancellationToken);
}
