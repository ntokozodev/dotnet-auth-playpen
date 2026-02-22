import { A } from "@solidjs/router";
import { For } from "solid-js";
import { usePagedScopes } from "@/queries/scopeQueries";

export function Scopes() {
  const query = usePagedScopes();

  return (
    <div class="space-y-4">
      <div class="flex items-center justify-between">
        <h1 class="text-2xl font-semibold">Scopes</h1>
        <A class="rounded bg-blue-700 px-3 py-2 text-sm font-semibold text-white" href="/scopes/create">
          + Create
        </A>
      </div>
      <For each={query.data?.pages.flatMap((p) => p.items) ?? []}>
        {(scope) => (
          <div class="flex items-start justify-between rounded border border-slate-200 bg-white p-4">
            <div>
              <div class="font-semibold">{scope.displayName}</div>
              <div class="text-sm text-slate-600">{scope.scopeName}</div>
            </div>
            <A class="text-sm text-blue-700" href={`/scopes/${scope.id}/edit`}>
              Edit
            </A>
          </div>
        )}
      </For>
      <button
        class="rounded bg-blue-700 px-3 py-2 text-white disabled:opacity-50"
        disabled={!query.hasNextPage || query.isFetchingNextPage}
        onClick={() => query.fetchNextPage()}
      >
        Load more
      </button>
    </div>
  );
}
