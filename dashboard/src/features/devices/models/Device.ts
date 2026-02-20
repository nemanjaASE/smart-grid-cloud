import type { DeviceType } from "../../../types/DeviceType";

export interface Device {
  deviceId: string;
  name: string;
  type: DeviceType;
  location: string;
  nominalPower: number;
  registeredAt: string;
}
