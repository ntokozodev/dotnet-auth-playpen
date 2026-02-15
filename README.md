# dotnet-auth-playpen API

ASP.NET Core 8 Web API + PostgreSQL that supports CRUD for `Application` and `Scope`.


## Project layout

The solution is now organized into layered projects:

- `AuthPlaypen.Api` - HTTP surface and startup/composition root only.
- `AuthPlaypen.Application` - DTOs and application services/use-cases.
- `AuthPlaypen.Data` - all EF Core persistence concerns.
- `AuthPlaypen.Domain` - domain entities and enums.

## Strict layer ownership

This repository follows strict layer ownership:

- `AuthPlaypen.Api`
  - Owns controllers, request/response wiring, DI registration, and host startup.
  - Must not own persistence artifacts (no migrations, no `DbContext` factory).
- `AuthPlaypen.Application`
  - Owns use-case orchestration and contracts.
  - Must not contain HTTP or EF Core infrastructure details.
- `AuthPlaypen.Data`
  - Owns persistence end-to-end: `DbContext`, entity mapping, design-time factory, and EF migrations.
- `AuthPlaypen.Domain`
  - Owns core business model and enums with no infrastructure dependencies.

In short: if a change is data/persistence-specific, it belongs in `AuthPlaypen.Data`.

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

## Migrations and design-time EF tooling

Under strict ownership, EF Core migrations and `AuthPlaypenDbContextFactory` should live in `AuthPlaypen.Data`.

Use EF commands with explicit target/startup projects:

```bash
dotnet ef migrations add <Name> \
  --project src/AuthPlaypen.Data \
  --startup-project src/AuthPlaypen.Api
```

```bash
dotnet ef database update \
  --project src/AuthPlaypen.Data \
  --startup-project src/AuthPlaypen.Api
```

At runtime, API remains the composition root and continues to apply migrations through `Database.Migrate()` during startup.
