namespace AuthPlaypen.Api.Entities;

public class ApplicationScopeEntity
{
    public Guid ApplicationId { get; set; }
    public ApplicationEntity Application { get; set; } = null!;

    public Guid ScopeId { get; set; }
    public ScopeEntity Scope { get; set; } = null!;
}
