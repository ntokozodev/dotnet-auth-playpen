import { A } from "@solidjs/router";
import { For } from "solid-js";
import { usePagedApplications } from "@/queries/applicationQueries";

export function Applications() {
  const query = usePagedApplications();

  return (
    <div class="space-y-4">
      <h1 class="text-2xl font-semibold">Applications</h1>
      <For each={query.data?.pages.flatMap((p) => p.items) ?? []}>
        {(app) => (
          <div class="rounded border border-slate-200 bg-white p-4">
            <div class="font-semibold">{app.displayName}</div>
            <div class="text-sm text-slate-600">{app.clientId}</div>
            <A class="text-sm text-blue-700" href={`/applications/${app.id}/edit`}>
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
