import { createContext } from "react";
import { Logger } from "./Logger";

export const LoggerContext = createContext<Logger | null>(null);
