import { HubConnectionState } from "@microsoft/signalr";

export const CONNECTION_CONFIG: Record<
  HubConnectionState,
  { label: string; className: string }
> = {
  [HubConnectionState.Connected]: {
    label: "● SYSTEM LIVE",
    className:
      "text-emerald-600 border-emerald-200 dark:bg-emerald-900/20 dark:text-emerald-400 dark:border-emerald-800",
  },
  [HubConnectionState.Connecting]: {
    label: "○ CONNECTING",
    className: "text-amber-600 border-amber-200 animate-pulse",
  },
  [HubConnectionState.Reconnecting]: {
    label: "↻ RECONNECTING",
    className: "text-amber-600 border-amber-200 animate-pulse",
  },
  [HubConnectionState.Disconnected]: {
    label: "✕ DISCONNECTED",
    className: "text-red-600 border-red-200",
  },
  [HubConnectionState.Disconnecting]: {
    label: "○ DISCONNECTING",
    className: "text-slate-600 border-slate-200",
  },
};
