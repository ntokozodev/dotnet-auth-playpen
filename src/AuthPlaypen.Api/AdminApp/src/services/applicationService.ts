import { http } from "./http";
import type { Application, CreateApplicationRequest, CursorPage } from "@/types/models";

export const applicationService = {
  getPaged: (cursor?: string, pageSize = 10) =>
    http<CursorPage<Application>>(`/applications?pageSize=${pageSize}${cursor ? `&cursor=${cursor}` : ""}`),
  getById: (id: string) => http<Application>(`/applications/${id}`),
  create: (payload: CreateApplicationRequest) => http<Application>("/applications", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: CreateApplicationRequest) =>
    http<Application>(`/applications/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  delete: (id: string) => http<void>(`/applications/${id}`, { method: "DELETE" }),
};
