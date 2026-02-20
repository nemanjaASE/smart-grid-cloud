import React from "react";
import type { Device } from "../models/Device";
import { DeviceRow } from "./DeviceRow";

export const DeviceList: React.FC<{ devices: Device[] }> = ({ devices }) => {
  return (
    <>
      {devices.map((device) => (
        <DeviceRow key={device.deviceId} device={device} />
      ))}
    </>
  );
};
