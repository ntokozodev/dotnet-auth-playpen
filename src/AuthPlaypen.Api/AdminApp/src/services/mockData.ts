import type { Application, Scope } from "@/types/models";

export const mockApplications = (): Application[] => [
  {
    id: "1",
    displayName: "Admin Portal",
    clientId: "admin-portal-client",
    clientSecret: "********",
    scopes: [
      {
        id: "1",
        displayName: "Read Users",
        scopeName: "users.read",
        description: "Allows reading user information",
      },
      {
        id: "2",
        displayName: "Write Users",
        scopeName: "users.write",
        description: "Allows creating and updating users",
      },
    ],
    flow: "AuthorizationWithPKCE",
    redirectUris: "https://admin.local/callback",
    postLogoutRedirectUris: "https://admin.local/logout",
  },
  {
    id: "2",
    displayName: "Mobile App",
    clientId: "mobile-client",
    clientSecret: "********",
    scopes: [
      {
        id: "3",
        displayName: "Read Reports",
        scopeName: "reports.read",
        description: "Allows viewing reports",
      },
    ],
    flow: "AuthorizationWithPKCE",
    redirectUris: "myapp://callback",
  },
];

export const MockScopes = (): Scope[] => [
  {
    id: "1",
    displayName: "Read Users",
    scopeName: "users.read",
    description: "Allows reading user information",
    applications: [],
  },
  {
    id: "2",
    displayName: "Write Users",
    scopeName: "users.write",
    description: "Allows creating and updating users",
    applications: [],
  },
  {
    id: "3",
    displayName: "Read Reports",
    scopeName: "reports.read",
    description: "Allows viewing reports",
    applications: [],
  },
];
