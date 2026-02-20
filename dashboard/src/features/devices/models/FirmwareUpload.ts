export interface FirmwareUploadRequest {
  deviceType: string;
  version: string;
  firmwareFile: File;
}
