import api from "../../../config/api";
import type { DeviceStatus } from "../models/DeviceStatus";

const BASE_PATH = "deviceStatuses";

export const getDeviceStatuses = async (): Promise<DeviceStatus[]> => {
  const response = await api.get(BASE_PATH);

  return response.data as DeviceStatus[];
};
