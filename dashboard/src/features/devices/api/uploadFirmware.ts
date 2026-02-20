import api from "../../../config/api";
import type { FirmwareUploadRequest } from "../models/FirmwareUpload";

export async function uploadFirmware(data: FirmwareUploadRequest) {
  const formData = new FormData();

  formData.append("deviceType", data.deviceType);
  formData.append("version", data.version);
  formData.append("firmwareFile", data.firmwareFile);

  const response = await api.post("/firmwares/upload", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
    },
  });

  if (!response.status) {
    throw new Error("Firmware upload failed");
  }
}
