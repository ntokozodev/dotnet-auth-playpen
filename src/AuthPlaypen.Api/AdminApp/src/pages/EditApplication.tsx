import { useParams } from "@solidjs/router";
import { createEffect, createSignal } from "solid-js";
import { MultiSelect } from "@/components/MultiSelect";
import { useApplications, useUpdateApplication } from "@/queries/applicationQueries";
import { useScopes } from "@/queries/scopeQueries";

export function EditApplication() {
  const params = useParams();
  const apps = useApplications();
  const scopes = useScopes();
  const update = useUpdateApplication();
  const [selectedScopeIds, setSelectedScopeIds] = createSignal<string[]>([]);

  const selectedApp = () => apps.data?.find((a) => a.id === params.id);

  createEffect(() => {
    if (selectedApp()) setSelectedScopeIds(selectedApp()!.scopes.map((s) => s.id));
  });

  return (
    <div class="space-y-4">
      <h1 class="text-2xl font-semibold">Edit Application</h1>
      <p>{selectedApp()?.displayName}</p>
      <MultiSelect
        label="Scopes"
        options={(scopes.data ?? []).map((s) => ({ id: s.id, label: `${s.displayName} (${s.scopeName})` }))}
        selected={selectedScopeIds()}
        onChange={setSelectedScopeIds}
      />
      <button
        class="rounded bg-blue-700 px-3 py-2 text-white"
        onClick={() => {
          const app = selectedApp();
          if (!app) return;
          update.mutate({ id: app.id, payload: { ...app, scopeIds: selectedScopeIds() } });
        }}
      >
        Save
      </button>
    </div>
  );
}
