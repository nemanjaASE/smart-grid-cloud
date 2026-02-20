import React from "react";

interface DeviceTypeBadgeProps {
  type: string;
}

export const DeviceTypeBadge: React.FC<DeviceTypeBadgeProps> = ({ type }) => {
  const baseClass = `
    inline-flex items-center justify-center 
    px-3 py-1 
    rounded-full 
    text-[11px] font-bold uppercase tracking-tight 
    border border-transparent 
    whitespace-nowrap transition-all duration-200
  `;

  const typeStyles: Record<string, string> = {
    SolarPanel:
      "bg-amber-500/10 text-amber-600 border-amber-500/20 hover:bg-amber-500/20",
    WindTurbine:
      "bg-sky-500/10 text-sky-600 border-sky-500/20 hover:bg-sky-500/20",
  };

  const defaultStyle = "bg-slate-500/10 text-slate-500 border-slate-500/20";

  if (type && !typeStyles[type]) {
    console.warn(`DeviceTypeBadge: Missing styles for type "${type}"`);
  }

  const formatLabel = (str: string) => str.replace(/([A-Z])/g, " $1").trim();

  return (
    <span className={`${baseClass} ${typeStyles[type] || defaultStyle}`}>
      {formatLabel(type)}
    </span>
  );
};
