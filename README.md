# dotnet-auth-playpen API

ASP.NET Core 8 Web API + PostgreSQL that supports CRUD for `Application` and `Scope`.

## Domain rules implemented

- Every application must reference at least one scope when created or updated.
- A scope can be global (applies to all applications) or application-specific.
  - `applications = []` in scope payload means **global scope**.
  - `applications = [app1, app2]` means **scope only for those applications**.
- Scope update/delete operations are blocked when they would leave any existing application with zero effective scopes.
- `redirectUris` and `postLogoutRedirectUris` are only valid when `flow` is `AuthorizationWithPKCE`.

## Run with Docker

```bash
docker compose up --build
```

API: `http://localhost:8080`
Swagger UI: `http://localhost:8080/swagger`

## API contracts

### ApplicationFlow
- `ClientCredentials`
- `AuthorizationWithPKCE`

### Application payload shape

```json
{
  "id": "guid",
  "displayName": "Admin App",
  "clientId": "admin-client",
  "clientSecret": "secret",
  "flow": "AuthorizationWithPKCE",
  "postLogoutRedirectUris": "https://example.com/logout",
  "redirectUris": "https://example.com/callback",
  "scopes": [
    {
      "id": "guid",
      "displayName": "Read Users",
      "scopeName": "users.read",
      "description": "Read user profile data"
    }
  ]
}
```

### Scope payload shape

```json
{
  "id": "guid",
  "displayName": "Read Users",
  "scopeName": "users.read",
  "description": "Read user profile data",
  "applications": []
}
```

`applications: []` means the scope is global.

## Migrations

EF Core migration is included in `src/AuthPlaypen.Api/Migrations` and automatically applied on startup via `Database.Migrate()`.
