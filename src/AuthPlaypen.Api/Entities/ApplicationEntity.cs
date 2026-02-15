namespace AuthPlaypen.Api.Entities;

public class ApplicationEntity
{
    public Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public ApplicationFlow Flow { get; set; }
    public string? PostLogoutRedirectUris { get; set; }
    public string? RedirectUris { get; set; }

    public ICollection<ApplicationScopeEntity> ApplicationScopes { get; set; } = new List<ApplicationScopeEntity>();
}
