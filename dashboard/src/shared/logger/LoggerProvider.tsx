import type { ReactNode } from "react";
import { Logger } from "./Logger";
import { ConsoleTransport } from "./transports/ConsoleTransport";
import { LogLevel } from "./types";
import { LoggerContext } from "./LoggerContext";

const isDev = import.meta.env.DEV;

const logger = new Logger(
  [new ConsoleTransport()],
  isDev ? LogLevel.DEBUG : LogLevel.WARN,
);

export const LoggerProvider = ({ children }: { children: ReactNode }) => (
  <LoggerContext.Provider value={logger}>{children}</LoggerContext.Provider>
);
