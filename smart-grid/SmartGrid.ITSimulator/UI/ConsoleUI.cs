using SmartGrid.ITSimulator.Enums;

namespace SmartGrid.ITSimulator.UI
{
    public class ConsoleUI
    {
        public static void PrintHeader()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("======================================");
            Console.WriteLine("   SMART GRID INVERTER SIMULATOR v1.0   ");
            Console.WriteLine("======================================\n");
            Console.ResetColor();
        }
        public static string GetDeviceNameInput()
        {
            Console.Write("[INPUT] Enter Device Friendly Name: ");
            return Console.ReadLine()?.Trim() ?? "Generic Inverter";
        }
        public static DeviceType GetDeviceTypeInput()
        {
            Console.WriteLine("[INPUT] Select Device Type:");
            Console.WriteLine("  1. SolarPanel");
            Console.WriteLine("  2. WindTurbine");
            Console.Write("Selection (1-2): ");

            string choice = Console.ReadLine() ?? "1";
            return choice switch
            {
                "1" => DeviceType.SolarPanel,
                "2" => DeviceType.WindTurbine,
                _ => DeviceType.SolarPanel
            };
        }
        public static double GetNominalPowerInput()
        {
            Console.Write("[INPUT] Enter Nominal Power (Watts): ");
            if (double.TryParse(Console.ReadLine(), out double nominalPower) && nominalPower > 0)
            {
                return nominalPower;
            }
            return 1000.0;
        }
        public static string GetLocationInput()
        {
            Console.Write("[INPUT] Enter Location (e.g., Belgrade_Plant_A): ");
            return Console.ReadLine()?.Trim() ?? "Unknown_Location";
        }
        public static void PrintStartMessage(string deviceName, string apiUrl)
        {
            Console.WriteLine("\n--------------------------------------");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[SYSTEM] Starting simulation for: {deviceName}");
            Console.WriteLine($"[SYSTEM] Target API: {apiUrl}");
            Console.ResetColor();
            Console.WriteLine("[SYSTEM] Press Ctrl+C to stop simulation.");
            Console.WriteLine("--------------------------------------\n");
        }

        public static void PrintSuccess(double currentPower, double nominalPower)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"[{timestamp}] [SENT] Power: {currentPower,8:F2}W | Load: {(currentPower / nominalPower) * 100,5:F1}%");
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] {message}");
            Console.ResetColor();
        }
        public static void PrintCritical(string message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"\n[CRITICAL ERROR] {message}");
            Console.ResetColor();
        }
    }
}
