import React, { useState } from "react";
import { uploadFirmware } from "../api/uploadFirmware";
import type { FirmwareUploadRequest } from "../models/FirmwareUpload";
import toast from "react-hot-toast";

interface FirmwareUploadModalProps {
  open: boolean;
  onClose: () => void;
}

export const FirmwareUploadModal: React.FC<FirmwareUploadModalProps> = ({
  open,
  onClose,
}) => {
  const [firmware, setFirmware] = useState<FirmwareUploadRequest>({
    deviceType: "",
    version: "",
    firmwareFile: null as unknown as File,
  });
  const [error, setError] = useState("");
  const [isUploading, setIsUploading] = useState(false);

  if (!open) return null;

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const selected = e.target.files?.[0];
    if (!selected) return;

    if (!selected.name.endsWith(".bin")) {
      setError("Firmware file must be a .bin file");
      return;
    }

    setError("");
    setFirmware((f) => ({ ...f, firmwareFile: selected }));
  };

  const handleSubmit = async () => {
    if (!firmware.deviceType || !firmware.version || !firmware.firmwareFile) {
      setError("All fields are required");
      return;
    }

    try {
      setIsUploading(true);
      await uploadFirmware(firmware as FirmwareUploadRequest);
      toast.success("Firmware successfully uploaded!");
      handleClose();
    } catch {
      toast.error("Upload failed.");
    } finally {
      setIsUploading(false);
    }
  };

  const handleClose = () => {
    setFirmware({
      deviceType: "",
      version: "",
      firmwareFile: null as unknown as File,
    });
    setError("");
    setIsUploading(false);
    onClose();
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm">
      <div className="w-full max-w-md rounded-2xl bg-slate-900 border border-slate-800 shadow-2xl p-6">
        {/* Header */}
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-lg font-bold text-white tracking-tight">
            Upload Firmware
          </h2>
          <button
            onClick={handleClose}
            className="text-slate-500 hover:text-slate-300 transition"
          >
            ✕
          </button>
        </div>

        {/* Form */}
        <div className="space-y-4">
          {/* Device Type */}
          <div>
            <label className="block text-xs font-bold uppercase tracking-widest text-slate-400 mb-1">
              Device Type
            </label>
            <select
              value={firmware.deviceType}
              onChange={(e) =>
                setFirmware((f) => ({ ...f, deviceType: e.target.value }))
              }
              className="w-full rounded-lg bg-slate-800 border border-slate-700 px-3 py-2 text-sm text-slate-200 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            >
              <option value="">Select device type</option>
              <option value="SolarPanel">Solar Panel</option>
              <option value="WindTurbine">Wind Turbine</option>
            </select>
          </div>

          {/* Version */}
          <div>
            <label className="block text-xs font-bold uppercase tracking-widest text-slate-400 mb-1">
              Firmware Version
            </label>
            <input
              type="text"
              placeholder="v1.2.3"
              value={firmware.version}
              onChange={(e) =>
                setFirmware((f) => ({ ...f, version: e.target.value }))
              }
              className="w-full rounded-lg bg-slate-800 border border-slate-700 px-3 py-2 text-sm text-slate-200 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            />
          </div>

          {/* File */}
          <div>
            <label className="block text-xs font-bold uppercase tracking-widest text-slate-400 mb-1">
              Firmware File (.bin)
            </label>
            <input
              type="file"
              accept=".bin"
              onChange={handleFileChange}
              className="w-full text-sm text-slate-400
                file:mr-4 file:rounded-lg file:border-0
                file:bg-indigo-600 file:px-4 file:py-2
                file:text-sm file:font-bold
                file:text-white hover:file:bg-indigo-500"
            />
          </div>

          {error && (
            <div className="text-xs text-red-400 font-medium">{error}</div>
          )}
        </div>

        {/* Actions */}
        <div className="flex justify-end gap-3 mt-6">
          <button
            onClick={onClose}
            className="px-4 py-2 text-sm font-bold text-slate-400 hover:text-slate-200"
          >
            Cancel
          </button>
          <button
            onClick={handleSubmit}
            disabled={isUploading}
            className="px-4 py-2 rounded-lg bg-indigo-600 hover:bg-indigo-500 text-white text-sm font-bold shadow-lg shadow-indigo-500/20"
          >
            {isUploading ? "Uploading..." : "Upload"}
          </button>
        </div>
      </div>
    </div>
  );
};
