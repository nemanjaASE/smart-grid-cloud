import { useContext } from "react";
import { LoggerContext } from "./LoggerContext";

export const useLogger = () => {
  const logger = useContext(LoggerContext);
  if (!logger) throw new Error("useLogger must be inside <LoggerProvider>");
  return logger;
};
