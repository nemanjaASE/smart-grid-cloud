import type { DeviceType } from "../../../types/DeviceType";
import type { FirmwareUploadStatus } from "../types/FirmwareUploadStatus";

export interface DeviceStatus {
  deviceId: string;
  deviceType: DeviceType;
  currentPower: number;
  loadPercentage: number;
  isOnline: boolean;
  isUnderperforming: boolean;
  isOverloaded: boolean;
  currentFirmwareVersion: string;
  targetFirmwareVersion: string;
  updateStatus: FirmwareUploadStatus;
}
