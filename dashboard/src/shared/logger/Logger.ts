import type { ITransport, LogEntry } from "./types";
import { LogLevel } from "./types";

export class Logger {
  private readonly transports: ITransport[];
  private readonly minLevel: LogLevel;

  constructor(transports: ITransport[], minLevel: LogLevel = LogLevel.DEBUG) {
    this.transports = transports;
    this.minLevel = minLevel;
  }

  private log(
    level: LogLevel,
    message: string,
    context?: Record<string, unknown>,
  ) {
    if (level < this.minLevel) return;

    const entry: LogEntry = {
      level,
      message,
      context,
      timestamp: new Date().toISOString(),
    };

    this.transports.forEach((t) => t.send(entry));
  }

  info(message: string, context?: Record<string, unknown>) {
    this.log(LogLevel.INFO, message, context);
  }
  warn(message: string, context?: Record<string, unknown>) {
    this.log(LogLevel.WARN, message, context);
  }
  error(message: string, context?: Record<string, unknown>) {
    this.log(LogLevel.ERROR, message, context);
  }
}
