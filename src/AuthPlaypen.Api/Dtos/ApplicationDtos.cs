using AuthPlaypen.Api.Entities;

namespace AuthPlaypen.Api.Dtos;

public record ScopeReferenceDto(Guid Id, string DisplayName, string ScopeName, string Description);

public record ApplicationDto(
    Guid Id,
    string DisplayName,
    string ClientId,
    string ClientSecret,
    ApplicationFlow Flow,
    string? PostLogoutRedirectUris,
    string? RedirectUris,
    IReadOnlyCollection<ScopeReferenceDto> Scopes);

public record CreateApplicationRequest(
    string DisplayName,
    string ClientId,
    string ClientSecret,
    ApplicationFlow Flow,
    string? PostLogoutRedirectUris,
    string? RedirectUris,
    IReadOnlyCollection<Guid> ScopeIds);

public record UpdateApplicationRequest(
    string DisplayName,
    string ClientId,
    string ClientSecret,
    ApplicationFlow Flow,
    string? PostLogoutRedirectUris,
    string? RedirectUris,
    IReadOnlyCollection<Guid> ScopeIds);
