import { A } from "@solidjs/router";
import { For } from "solid-js";

type BreadcrumbItem = {
  href?: string;
  label: string;
};

type BreadcrumbsProps = {
  items: BreadcrumbItem[];
};

export function Breadcrumbs(props: BreadcrumbsProps) {
  return (
    <nav aria-label="Breadcrumb" class="text-sm text-slate-500">
      <ol class="flex flex-wrap items-center gap-2">
        <For each={props.items}>
          {(item, index) => (
            <li class="flex items-center gap-2">
              {index() > 0 && <span class="text-slate-400">&gt;</span>}
              {item.href ? <A class="text-slate-600 hover:text-blue-700" href={item.href}>{item.label}</A> : <span class="font-medium text-slate-900">{item.label}</span>}
            </li>
          )}
        </For>
      </ol>
    </nav>
  );
}
