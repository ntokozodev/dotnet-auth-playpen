import { createInfiniteQuery, createMutation, useQuery, useQueryClient } from "@tanstack/solid-query";
import { applicationService } from "@/services/applicationService";

export const applicationKeys = {
  all: ["applications"] as const,
  lists: () => [...applicationKeys.all, "list"] as const,
  paged: () => [...applicationKeys.all, "paged"] as const,
  detail: (id: string) => [...applicationKeys.all, "detail", id] as const,
};

export function useApplications() {
  return useQuery(() => ({
    queryKey: applicationKeys.lists(),
    queryFn: applicationService.getAll,
  }));
}

export function usePagedApplications() {
  return createInfiniteQuery(() => ({
    queryKey: applicationKeys.paged(),
    queryFn: ({ pageParam }) => applicationService.getPaged(pageParam),
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined as string | undefined,
  }));
}

export function useCreateApplication() {
  const queryClient = useQueryClient();
  return createMutation(() => ({
    mutationFn: applicationService.create,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: applicationKeys.all }),
  }));
}

export function useUpdateApplication() {
  const queryClient = useQueryClient();
  return createMutation(() => ({
    mutationFn: ({ id, payload }: { id: string; payload: Parameters<typeof applicationService.update>[1] }) =>
      applicationService.update(id, payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: applicationKeys.all }),
  }));
}
