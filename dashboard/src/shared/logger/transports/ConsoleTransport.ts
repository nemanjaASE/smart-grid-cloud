import { LogLevel } from "./../types";
import type { ITransport, LogEntry } from "../types";

export class ConsoleTransport implements ITransport {
  send(entry: LogEntry): void {
    const msg = `[${entry.timestamp}] ${entry.message}`;
    const fn =
      entry.level >= LogLevel.ERROR
        ? console.error
        : entry.level >= LogLevel.WARN
          ? console.warn
          : console.log;
    fn(msg, entry.context ?? "");
  }
}
