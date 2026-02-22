import { useParams } from "@solidjs/router";
import { createEffect, createSignal } from "solid-js";
import { MultiSelect } from "@/components/MultiSelect";
import { useApplications } from "@/queries/applicationQueries";
import { useScopes, useUpdateScope } from "@/queries/scopeQueries";

export function EditScope() {
  const params = useParams();
  const scopes = useScopes();
  const apps = useApplications();
  const update = useUpdateScope();
  const [selectedAppIds, setSelectedAppIds] = createSignal<string[]>([]);

  const selectedScope = () => scopes.data?.find((s) => s.id === params.id);

  createEffect(() => {
    if (selectedScope()) setSelectedAppIds(selectedScope()!.applications.map((a) => a.id));
  });

  return (
    <div class="space-y-4">
      <h1 class="text-2xl font-semibold">Edit Scope</h1>
      <p>{selectedScope()?.displayName}</p>
      <MultiSelect
        label="Applications"
        options={(apps.data ?? []).map((a) => ({ id: a.id, label: `${a.displayName} (${a.clientId})` }))}
        selected={selectedAppIds()}
        onChange={setSelectedAppIds}
      />
      <button
        class="rounded bg-blue-700 px-3 py-2 text-white"
        onClick={() => {
          const scope = selectedScope();
          if (!scope) return;
          update.mutate({ id: scope.id, payload: { ...scope, applicationIds: selectedAppIds() } });
        }}
      >
        Save
      </button>
    </div>
  );
}
