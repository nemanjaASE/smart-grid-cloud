import React, { useEffect, useState } from "react";

import { DeviceList } from "../components/DeviceList";
import { PageLayout } from "../../../layouts/PageLayout";
import { getDevices } from "../api/getDevices";
import type { Device } from "../models/Device";
import { FirmwareUploadModal } from "../components/FirmwareUploadModal";

const DevicesPage: React.FC = () => {
  const [devices, setDevices] = useState<Device[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isFirmwareModalOpen, setIsFirmwareModalOpen] = useState(false);

  useEffect(() => {
    getDevices()
      .then(setDevices)
      .finally(() => setIsLoading(false));
  }, []);

  if (isLoading)
    return (
      <div className="flex justify-center items-center p-20">
        {" "}
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-cyan-500"></div>{" "}
        <span className="ml-3 text-slate-400 font-medium">
          {" "}
          Loading devices...{" "}
        </span>{" "}
      </div>
    );

  return (
    <PageLayout>
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-white tracking-tight">
            Device Inventory
          </h1>
          <p className="text-slate-400 text-sm">
            Overview and management of all registered devices in the system.
          </p>
        </div>

        <button
          className="bg-indigo-600 hover:bg-indigo-500 text-white px-4 py-2 rounded-lg font-medium transition-colors shadow-lg shadow-indigo-500/20 text-sm"
          onClick={() => setIsFirmwareModalOpen(true)}
        >
          Upload New Firmware
        </button>
      </div>

      {/* Table */}
      <div className="bg-white/70 dark:bg-slate-800/50 backdrop-blur-md rounded-2xl shadow-xl shadow-slate-200/50 dark:shadow-none border border-white dark:border-slate-700 overflow-hidden">
        {isLoading ? (
          <div className="flex justify-center items-center p-20">
            <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-cyan-500"></div>
            <span className="ml-3 text-slate-400 font-medium">
              Loading devices...
            </span>
          </div>
        ) : (
          <table className="w-full border-separate border-spacing-0">
            <thead className="bg-slate-50/50 dark:bg-slate-900/30">
              <tr>
                <th className="px-6 py-4 align-middle text-left text-xs font-bold text-slate-400 uppercase tracking-widest">
                  Name / ID
                </th>
                <th className="px-6 py-4 align-middle text-center text-xs font-bold text-slate-400 uppercase tracking-widest">
                  Device Type
                </th>
                <th className="px-6 py-4 align-middle text-left text-xs font-bold text-slate-400 uppercase tracking-widest">
                  Location
                </th>
                <th className="px-6 py-4 align-middle text-right text-xs font-bold text-slate-400 uppercase tracking-widest">
                  Nominal Power
                </th>
                <th className="px-6 py-4 align-middle text-right text-xs font-bold text-slate-400 uppercase tracking-widest">
                  Registered
                </th>
              </tr>
            </thead>

            <tbody className="divide-y divide-slate-100 dark:divide-slate-700">
              <DeviceList devices={devices} />
            </tbody>
          </table>
        )}
      </div>
      <FirmwareUploadModal
        open={isFirmwareModalOpen}
        onClose={() => setIsFirmwareModalOpen(false)}
      />
    </PageLayout>
  );
};

export default DevicesPage;
