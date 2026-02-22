export type StrictOmit<T, K extends keyof T> = Omit<T, K>;

export type ApplicationFlow = "ClientCredentials" | "AuthorizationWithPKCE";

export type Application = {
  id: string;
  displayName: string;
  clientId: string;
  clientSecret: string;
  scopes: ScopeReference[];
  flow: ApplicationFlow;
  postLogoutRedirectUris?: string;
  redirectUris?: string;
};

export type Scope = {
  id: string;
  displayName: string;
  scopeName: string;
  description: string;
  applications: ApplicationReference[];
};

export type ScopeReference = StrictOmit<Scope, "applications">;
export type ApplicationReference = StrictOmit<Application, "scopes">;

export type CursorPage<T> = {
  items: T[];
  nextCursor?: string;
};

export type CreateApplicationRequest = Omit<Application, "id" | "scopes"> & { scopeIds: string[] };
export type CreateScopeRequest = Omit<Scope, "id" | "applications"> & { applicationIds: string[] };
