import React from "react";
import { useDeviceSignalR } from "../hooks/useDeviceSignalR";
import { DeviceRow } from "../components/DeviceStatusRow";
import { PageLayout } from "../../../layouts/PageLayout";
import { HubConnectionState } from "@microsoft/signalr";
import { CONNECTION_CONFIG } from "../constants/connectionConfig";
import { DEVICE_STATUS_UPDATE_EVENT } from "../constants/events";

const Dashboard: React.FC = () => {
  const { devices, state } = useDeviceSignalR(DEVICE_STATUS_UPDATE_EVENT);
  const isConnected = state === HubConnectionState.Connected;
  const { label, className } = CONNECTION_CONFIG[state];

  return (
    <PageLayout>
      {/* Header Section */}
      <header className="flex justify-between items-end mb-10">
        <div
          className={`px-4 py-2 rounded-full text-xs font-bold tracking-wider border shadow-sm transition-all
            ${
              isConnected
                ? "bg-emerald-50 text-emerald-600 border-emerald-200 dark:bg-emerald-900/20 dark:text-emerald-400 dark:border-emerald-800"
                : "bg-amber-50 text-amber-600 border-amber-200 dark:bg-amber-900/20 dark:text-amber-400 dark:border-amber-800"
            }`}
        >
          <div className={`text-xs font-bold transition-all ${className}`}>
            {label}
          </div>
        </div>
      </header>

      {/* Table Section */}
      <div className="bg-white/70 dark:bg-slate-800/50 backdrop-blur-md rounded-2xl shadow-xl shadow-slate-200/50 dark:shadow-none border border-white dark:border-slate-700 overflow-hidden">
        <table className="w-full border-separate border-spacing-0">
          <thead className="bg-slate-50/50 dark:bg-slate-900/30">
            <tr>
              <th className="px-6 py-4 align-middle text-left text-xs font-bold text-slate-400 uppercase tracking-widest">
                Device
              </th>
              <th className="px-6 py-4 align-middle text-center text-xs font-bold text-slate-400 uppercase tracking-widest">
                Device Type
              </th>
              <th className="px-6 py-4 align-middle text-right text-xs font-bold text-slate-400 uppercase tracking-widest">
                Power Output
              </th>
              <th className="px-6 py-4 align-middle text-center text-xs font-bold text-slate-400 uppercase tracking-widest">
                Load Info
              </th>
              <th className="px-6 py-4 align-middle text-left text-xs font-bold text-slate-400 uppercase tracking-widest">
                Firmware
              </th>
              <th className="px-6 py-4 align-middle text-center text-xs font-bold text-slate-400 uppercase tracking-widest">
                Status
              </th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 dark:divide-slate-700">
            {devices.length === 0 ? (
              <tr>
                <td
                  colSpan={6}
                  className="px-6 py-20 text-center text-slate-400 italic font-light"
                >
                  Waiting for incoming device telemetry...
                </td>
              </tr>
            ) : (
              devices.map((d) => <DeviceRow key={d.deviceId} device={d} />)
            )}
          </tbody>
        </table>
      </div>
    </PageLayout>
  );
};

export default Dashboard;
