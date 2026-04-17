import { useSignalR } from "./../../../shared/signalr/useSignalR";
import { useEffect, useState } from "react";
import { HubConnectionState } from "@microsoft/signalr";
import { useLogger } from "../../../shared/logger/useLogger";
import type { DeviceStatus } from "../models/DeviceStatus";
import { getDeviceStatuses } from "../api/getDeviceStatuses";

export const useDeviceSignalR = (eventName: string) => {
  const logger = useLogger();
  const { connection, state } = useSignalR();

  const [devices, setDevices] = useState<DeviceStatus[]>([]);

  useEffect(() => {
    if (!connection) return;

    let isMounted = true;

    const fetchInitialData = async () => {
      try {
        const initial = await getDeviceStatuses();
        if (isMounted) {
          setDevices(initial);
          logger.info("Initial device statuses fetched.");
        }
      } catch (err) {
        logger.error("Failed to fetch initial devices.", { err });
      }
    };

    if (state === HubConnectionState.Connected) {
      fetchInitialData();
    }
    connection.on(eventName, (updatedDevice: DeviceStatus) => {
      if (!isMounted) return;

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

    return () => {
      isMounted = false;
      connection.off(eventName);
    };
  }, [connection, state, eventName, logger]);

  return { devices, state };
};
