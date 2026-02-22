import { http } from "./http";
import type { Scope, CreateScopeRequest, CursorPage } from "@/types/models";

export const scopeService = {
  getAll: () => http<Scope[]>("/scopes"),
  getPaged: (cursor?: string, pageSize = 10) =>
    http<CursorPage<Scope>>(`/scopes/paged?pageSize=${pageSize}${cursor ? `&cursor=${cursor}` : ""}`),
  getById: (id: string) => http<Scope>(`/scopes/${id}`),
  create: (payload: CreateScopeRequest) => http<Scope>("/scopes", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: CreateScopeRequest) =>
    http<Scope>(`/scopes/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  delete: (id: string) => http<void>(`/scopes/${id}`, { method: "DELETE" }),
};
