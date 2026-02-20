import React from "react";

interface PageLayoutProps {
  children: React.ReactNode;
}

export const PageLayout: React.FC<PageLayoutProps> = ({ children }) => {
  return (
    <div className="p-8 border-4 border-gray-500/20 rounded-2xl transition-colors duration-300 bg-slate-50 dark:bg-slate-900">
      {children}
    </div>
  );
};
