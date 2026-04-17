import { createContext } from "react";
import type { SignalRContextState } from "./types";

export const SignalRContext = createContext<SignalRContextState | null>(null);
