import React from "react";
import type { DeviceStatus } from "../models/DeviceStatus";
import { StatusBadge } from "../../../components/StatusBadge";
import { DeviceTypeBadge } from "../../../components/DeviceTypeBadge";

export const DeviceRow: React.FC<{ device: DeviceStatus }> = ({ device }) => {
  return (
    <tr
      className={`group border-b border-slate-800 transition-colors
        hover:bg-slate-800/40
        ${device.isOverloaded ? "bg-red-900/10" : ""}
      `}
    >
      {/* Device ID & Status */}
      <td className="px-6 py-4 align-middle text-left">
        <div className="flex items-center gap-4">
          <div
            className={`w-2.5 h-2.5 rounded-full relative shrink-0 ${
              device.isOnline
                ? "bg-emerald-500 shadow-[0_0_8px_rgba(16,185,129,0.5)]"
                : "bg-red-500"
            }`}
          >
            {device.isOnline && (
              <span className="absolute inset-0 rounded-full bg-emerald-500 animate-ping opacity-75" />
            )}
          </div>

          <div className="flex flex-col justify-center">
            <span className="text-sm font-bold text-slate-200 tracking-tight">
              {device.deviceId.slice(0, 8)}
            </span>
            <span className="text-[10px] text-slate-500 font-bold uppercase tracking-widest mt-0.5">
              ID Code
            </span>
          </div>
        </div>
      </td>

      {/* Type */}
      <td className="px-6 py-4 align-middle text-center">
        <DeviceTypeBadge type={device.deviceType} />
      </td>

      {/* Power */}
      <td className="px-6 py-4 align-middle text-right whitespace-nowrap">
        <span className="font-mono font-bold text-slate-200 text-[15px]">
          {device.currentPower.toLocaleString()}
        </span>
        <span className="text-[15px] text-indigo-500 font-bold ml-1">W</span>
      </td>

      {/* Load (CENTER) */}
      <td className="px-6 py-4 align-middle text-center">
        <div className="w-32 mx-auto">
          <div className="flex justify-between mb-1 text-[9px] font-black uppercase tracking-widest text-slate-500">
            <span>Utilization</span>
            <span className={device.loadPercentage > 85 ? "text-red-400" : ""}>
              {device.loadPercentage}%
            </span>
          </div>

          <div className="h-1.5 w-full bg-slate-800 rounded-full overflow-hidden">
            <div
              className={`h-full rounded-full transition-all duration-700 ${
                device.loadPercentage > 85
                  ? "bg-linear-to-r from-orange-500 to-red-600"
                  : "bg-linear-to-r from-indigo-500 to-indigo-400"
              }`}
              style={{ width: `${device.loadPercentage}%` }}
            />
          </div>
        </div>
      </td>

      {/* Firmware */}
      <td className="px-6 py-4 align-middle text-left">
        <div className="flex flex-col justify-center">
          <span className="text-sm font-medium text-slate-400">
            {device.currentFirmwareVersion}
          </span>
          {device.targetFirmwareVersion && (
            <span className="text-[10px] text-indigo-400 font-bold uppercase tracking-wider mt-0.5">
              → {device.targetFirmwareVersion}
            </span>
          )}
        </div>
      </td>

      {/* Update Status */}
      <td className="px-6 py-4 align-middle text-center">
        <StatusBadge status={device.updateStatus} />
      </td>
    </tr>
  );
};
