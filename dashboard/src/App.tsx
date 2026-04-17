import { RouterProvider } from "react-router-dom";
import "./App.css";
import { router } from "./routes";
import { LoggerProvider } from "./shared/logger/LoggerProvider";
import { SignalRProvider } from "./shared/signalr/SignalRProvider";
import { CONFIG } from "./config/config";

function App() {
  return (
    <LoggerProvider>
      <SignalRProvider
        hubUrl={CONFIG.HUB_URL}
        reconnectTimeoutMs={CONFIG.SIGNALR_RECONNECT_INTERVAL}
      >
        <RouterProvider router={router} />
      </SignalRProvider>
    </LoggerProvider>
  );
}

export default App;
