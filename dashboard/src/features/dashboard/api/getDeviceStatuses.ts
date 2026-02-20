import api from "../../../config/api";
import type { DeviceStatus } from "../models/DeviceStatus";

export const getDeviceStatuses = async (): Promise<DeviceStatus[]> => {
  const response = await api.get("/deviceStatuses");
  if (!response.status) {
    throw new Error("Device statuses read failed");
  }

  return response.data as DeviceStatus[];
};
