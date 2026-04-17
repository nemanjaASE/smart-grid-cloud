const API_BASE_URL = import.meta.env.VITE_API_URL || "https://localhost:7262";
const HUB_NAME = import.meta.env.VITE_HUB_NAME || "device-status-hub";
const SIGNALR_RECONNECT_INTERVAL =
  import.meta.env.SIGNALR_RECONNECT_INTERVAL || 5000;

export const CONFIG = {
  API_BASE_URL: `${API_BASE_URL}/api`,
  HUB_URL: `${API_BASE_URL}/${HUB_NAME}`,
  SIGNALR_RECONNECT_INTERVAL: SIGNALR_RECONNECT_INTERVAL,
};
