import { createBrowserRouter } from "react-router-dom";
import MainLayout from "./layouts/MainLayout";
import Dashboard from "./features/dashboard/pages/Dashboard";
import DevicesPage from "./features/devices/pages/DevicesPage";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <MainLayout />,
    children: [
      { index: true, element: <Dashboard /> },
      { path: "devices", element: <DevicesPage /> },
      //   { path: "about", element: <AboutPage /> },
      //   { path: "contact", element: <ContactPage /> },
    ],
  },
]);
