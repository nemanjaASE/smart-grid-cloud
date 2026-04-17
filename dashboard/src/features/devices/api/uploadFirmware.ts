import api from "../../../config/api";
import type { FirmwareUploadRequest } from "../models/FirmwareUpload";

const BASE_PATH = "/firmwares";

export async function uploadFirmware(
  data: FirmwareUploadRequest,
): Promise<void> {
  const formData = new FormData();

  formData.append("deviceType", data.deviceType);
  formData.append("version", data.version);
  formData.append("firmwareFile", data.firmwareFile);

  await api.post(`${BASE_PATH}/upload`, formData);
}
