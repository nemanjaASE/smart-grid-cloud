import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export const createSignalRConnection = (hubUrl: string) => {
  return new HubConnectionBuilder()
    .withUrl(hubUrl)
    .withAutomaticReconnect()
    .configureLogging(LogLevel.None)
    .build();
};
