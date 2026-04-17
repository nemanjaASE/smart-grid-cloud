import api from "../../../config/api";
import type { Device } from "../models/Device";

const BASE_PATH = "/devices";

export const getDevices = async (): Promise<Device[]> => {
  const response = await api.get(BASE_PATH);

  return response.data as Device[];
};
