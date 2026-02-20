import api from "../../../config/api";
import type { Device } from "../models/Device";

export const getDevices = async (): Promise<Device[]> => {
  const response = await api.get("/devices");
  if (!response.status) {
    throw new Error("Devices read failed");
  }

  return response.data as Device[];
};
