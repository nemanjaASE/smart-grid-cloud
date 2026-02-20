import { useEffect, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { DeviceStatus } from "../models/DeviceStatus";
import { getDeviceStatuses } from "../api/getDeviceStatuses";

export const useDeviceSignalR = (hubUrl: string) => {
  const [devices, setDevices] = useState<DeviceStatus[]>([]);
  const [connection, setConnection] = useState<signalR.HubConnection | null>(
    null,
  );
  const [state, setState] = useState<"Connected" | "Connecting">("Connecting");

  useEffect(() => {
    getDeviceStatuses()
      .then(setDevices)
      .catch((err) => console.error("Failed to fetch devices: ", err));
  }, []);

  useEffect(() => {
    const conn = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    setConnection(conn);
  }, [hubUrl]);

  useEffect(() => {
    if (!connection) return;

    const startConnection = async () => {
      try {
        await connection.start();
        setState("Connected");

        connection.on("ReceiveStatusUpdate", (updatedDevice: DeviceStatus) => {
          setDevices((prev) => {
            const index = prev.findIndex(
              (d) => d.deviceId === updatedDevice.deviceId,
            );
            if (index !== -1) {
              const newDevices = [...prev];
              newDevices[index] = updatedDevice;
              return newDevices;
            }
            return [updatedDevice, ...prev];
          });
        });
      } catch (err) {
        console.error("SignalR start error:", err);
        setState("Connecting");
      }
    };

    startConnection();

    return () => {
      connection.stop();
    };
  }, [connection]);

  return { devices, state };
};
