import React, { useEffect, useState } from "react";
import { HubConnectionState } from "@microsoft/signalr";
import { SignalRContext } from "./SignalRContext";
import { createSignalRConnection } from "./signalr";
import { useLogger } from "../logger/useLogger";

interface Props {
  hubUrl: string;
  reconnectTimeoutMs?: number;
  children: React.ReactNode;
}

export const SignalRProvider: React.FC<Props> = ({
  hubUrl,
  reconnectTimeoutMs = 5000,
  children,
}) => {
  const logger = useLogger();
  const [state, setState] = useState<HubConnectionState>(
    HubConnectionState.Disconnected,
  );

  const [connection] = useState(() => createSignalRConnection(hubUrl));

  useEffect(() => {
    connection.onreconnecting(() => {
      setState(HubConnectionState.Reconnecting);
      logger.warn("SignalR: Reconnecting...");
    });
    connection.onreconnected(() => {
      setState(HubConnectionState.Connected);
      logger.info("SignalR: Reconnected successfully.");
    });
    connection.onclose(() => {
      setState(HubConnectionState.Disconnected);
      logger.error("SignalR: Connection closed.");
    });

    const start = async () => {
      if (connection.state === HubConnectionState.Disconnected) {
        try {
          await connection.start();
          setState(HubConnectionState.Connected);
          logger.info("SignalR Provider: Connected");
        } catch (err) {
          logger.error("SignalR Provider: Start failed", { err });
          setTimeout(start, reconnectTimeoutMs);
        }
      }
    };

    start();

    return () => {
      connection.off("onreconnecting");
      connection.off("onreconnected");
      connection.off("onclose");
      connection.stop();
    };
  }, [connection, logger, reconnectTimeoutMs]);

  return (
    <SignalRContext.Provider value={{ connection, state }}>
      {children}
    </SignalRContext.Provider>
  );
};
