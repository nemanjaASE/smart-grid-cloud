import React from "react";
import { Toaster } from "react-hot-toast";
import { Link, Outlet } from "react-router-dom";

const MainLayout: React.FC = () => {
  return (
    // bg-slate-950 je baza za ceo ekran
    <div className="flex flex-col min-h-screen bg-slate-950 text-slate-200">
      {/* Header - Glassmorphism efekt */}
      <header className="sticky top-0 z-50 bg-slate-900/80 backdrop-blur-md border-b border-slate-800 shadow-xl">
        <div className="container mx-auto flex items-center justify-between p-4">
          <Link to="/" className="group">
            <h1 className="text-xl font-black bg-linear-to-r from-indigo-400 to-cyan-400 bg-clip-text text-transparent group-hover:from-indigo-300 group-hover:to-cyan-300 transition-all">
              GRID.CLOUD
            </h1>
          </Link>
          <nav>
            <ul className="flex space-x-8">
              <li>
                <Link
                  to="/devices"
                  className="text-sm font-bold tracking-wide text-slate-400 hover:text-cyan-400 transition-colors duration-200"
                >
                  DEVICES
                </Link>
              </li>
            </ul>
          </nav>
        </div>
      </header>

      {/* Main content - Ovde Outlet renderuje Dashboard */}
      <main className="flex-1 container mx-auto p-6">
        <Toaster position="top-right" reverseOrder={false} />
        <Outlet />
      </main>

      {/* Footer */}
      <footer className="border-t border-slate-900 bg-slate-950 py-6 mt-auto">
        <div className="container mx-auto text-center">
          <p className="text-slate-600 text-[10px] font-bold tracking-widest uppercase">
            © 2026 Cloud vezbe
          </p>
        </div>
      </footer>
    </div>
  );
};

export default MainLayout;
