import { createInfiniteQuery, createMutation, useQuery, useQueryClient } from "@tanstack/solid-query";
import { scopeService } from "@/services/scopeService";

export const scopeKeys = {
  all: ["scopes"] as const,
  lists: () => [...scopeKeys.all, "list"] as const,
  paged: () => [...scopeKeys.all, "paged"] as const,
};

export function useScopes() {
  return useQuery(() => ({
    queryKey: scopeKeys.lists(),
    queryFn: scopeService.getAll,
  }));
}

export function usePagedScopes() {
  return createInfiniteQuery(() => ({
    queryKey: scopeKeys.paged(),
    queryFn: ({ pageParam }) => scopeService.getPaged(pageParam),
    getNextPageParam: (lastPage) => lastPage.nextCursor,
    initialPageParam: undefined as string | undefined,
  }));
}

export function useCreateScope() {
  const queryClient = useQueryClient();
  return createMutation(() => ({
    mutationFn: scopeService.create,
    onSuccess: () => queryClient.invalidateQueries({ queryKey: scopeKeys.all }),
  }));
}

export function useUpdateScope() {
  const queryClient = useQueryClient();
  return createMutation(() => ({
    mutationFn: ({ id, payload }: { id: string; payload: Parameters<typeof scopeService.update>[1] }) =>
      scopeService.update(id, payload),
    onSuccess: () => queryClient.invalidateQueries({ queryKey: scopeKeys.all }),
  }));
}
