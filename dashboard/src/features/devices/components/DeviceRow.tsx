import React from "react";
import type { Device } from "../models/Device";
import { DeviceTypeBadge } from "../../../components/DeviceTypeBadge";

export const DeviceRow: React.FC<{ device: Device }> = ({ device }) => {
  return (
    <tr className="hover:bg-slate-800/40 transition-all duration-200 group">
      {/* Name & ID */}
      <td className="px-6 py-4 align-middle">
        <div className="flex flex-col justify-center">
          <span className="text-[15px] font-bold text-slate-200 tracking-tight group-hover:text-cyan-400 transition-colors truncate">
            {device.name}
          </span>
          <span className="text-[10px] text-slate-500 font-bold uppercase tracking-widest mt-0.5 truncate">
            {device.deviceId.split("-")[0]}
          </span>
        </div>
      </td>

      {/* Type */}
      <td className="px-6 py-4 align-middle text-center">
        <DeviceTypeBadge type={device.type} />
      </td>

      {/* Location */}
      <td className="px-6 py-4 align-right">
        <div className="flex items-center gap-2 text-[15px] text-slate-400 font-medium">
          <span className="text-slate-600 flex-0">📍</span>
          <span className="truncate">{device.location}</span>
        </div>
      </td>

      {/* Nominal Power */}
      <td className="px-6 py-4 align-middle text-right whitespace-nowrap">
        <span className="font-mono font-bold text-slate-200 text-[15px]">
          {device.nominalPower.toLocaleString()}
        </span>
        <span className="text-[15px] text-indigo-500 font-bold ml-1">W</span>
      </td>

      {/* Registered Date */}
      <td className="px-6 py-4 align-middle text-right whitespace-nowrap">
        <span className="text-[14px] font-medium text-slate-500 uppercase tracking-wide">
          {new Date(device.registeredAt).toLocaleDateString("en-GB", {
            day: "2-digit",
            month: "short",
            year: "numeric",
          })}
        </span>
      </td>
    </tr>
  );
};
