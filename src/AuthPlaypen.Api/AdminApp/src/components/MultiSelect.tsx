import { For } from "solid-js";

type Option = { id: string; label: string };

export function MultiSelect(props: {
  options: Option[];
  selected: string[];
  onChange: (ids: string[]) => void;
  label: string;
}) {
  return (
    <label class="flex flex-col gap-2 text-sm text-slate-700">
      {props.label}
      <select
        multiple
        class="min-h-28 rounded border border-slate-300 bg-white p-2"
        value={props.selected}
        onChange={(e) => {
          const values = Array.from(e.currentTarget.selectedOptions).map((o) => o.value);
          props.onChange(values);
        }}
      >
        <For each={props.options}>{(option) => <option value={option.id}>{option.label}</option>}</For>
      </select>
    </label>
  );
}
