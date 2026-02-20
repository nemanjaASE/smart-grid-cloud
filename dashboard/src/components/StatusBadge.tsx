import React from "react";

interface StatusBadgeProps {
  status: string;
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
  const baseClass = `
    inline-flex items-center justify-center 
    px-3 py-1 
    rounded-full 
    text-[11px] font-bold uppercase tracking-tight 
    border 
    whitespace-nowrap transition-all duration-300
  `;

  const statusStyles: Record<string, string> = {
    UpToDate: "bg-emerald-500/10 text-emerald-600 border-emerald-500/20",
    Updating:
      "bg-indigo-500/10 text-indigo-600 border-indigo-500/20 animate-pulse",
    UpdateFailed: "bg-rose-500/10 text-rose-600 border-rose-500/20",
    PendingUpdate: "bg-orange-500/10 text-orange-600 border-orange-500/20",
  };

  const defaultStyle = "bg-slate-500/10 text-slate-500 border-slate-500/20";

  if (status && !statusStyles[status]) {
    console.warn(`StatusBadge: Received an undefined status type: "${status}"`);
  }

  const formatStatus = (str: string) => str.replace(/([A-Z])/g, " $1").trim();

  return (
    <span className={`${baseClass} ${statusStyles[status] || defaultStyle}`}>
      {formatStatus(status)}
    </span>
  );
};
