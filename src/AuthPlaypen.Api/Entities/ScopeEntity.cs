namespace AuthPlaypen.Api.Entities;

public class ScopeEntity
{
    public Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required string ScopeName { get; set; }
    public required string Description { get; set; }
    public bool IsGlobal { get; set; }

    public ICollection<ApplicationScopeEntity> ApplicationScopes { get; set; } = new List<ApplicationScopeEntity>();
}
