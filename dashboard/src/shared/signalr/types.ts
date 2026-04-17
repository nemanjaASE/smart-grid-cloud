import { HubConnectionState, HubConnection } from "@microsoft/signalr";

export interface SignalRContextState {
  connection: HubConnection | null;
  state: HubConnectionState;
}
