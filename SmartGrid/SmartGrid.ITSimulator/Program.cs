using Microsoft.Extensions.Configuration;
using SmartGrid.ITSimulator.Enums;
using SmartGrid.ITSimulator.Services;
using SmartGrid.ITSimulator.UI;
using SmartGrid.ITSimulator.Models;


// Load configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

// Get settings
var baseApiUrl = configuration["SimulatorSettings:BaseApiUrl"]
    ?? throw new InvalidOperationException("Device URL is not configured");
var delayMs = int.Parse(configuration["SimulatorSettings:DelayMilliseconds"] ?? "10000");
var maxVariation = double.Parse(configuration["SimulatorSettings:MaxPowerVariation"] ?? "50");

// Initialize services
ConsoleUI.PrintHeader();

string deviceName = ConsoleUI.GetDeviceNameInput();
DeviceType deviceType = ConsoleUI.GetDeviceTypeInput();
double nominalPower = ConsoleUI.GetNominalPowerInput();
string location = ConsoleUI.GetLocationInput();
string currentVersion = "V1.0.0";

using var httpClient = new HttpClient { BaseAddress = new Uri(baseApiUrl)};

var deviceClient = new DeviceClient(httpClient);

string? deviceId =
    await deviceClient.RegisterDeviceAsync(
        new DeviceDTO
        {
            DeviceName = deviceName,
            DeviceType = deviceType,
            Location = location,
            NominalPower = nominalPower
        }
    );

if (deviceId == null)
{
    ConsoleUI.PrintCritical("Registration failed!");
    return;
}

var simulator = new SimulatorService(maxVariation);
var publisher = new TelemetryPublisher(httpClient);

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine($"\n[SYSTEM] Registering device '{deviceName}' on API...");
Console.ResetColor();

ConsoleUI.PrintStartMessage(deviceName, baseApiUrl + "/api/ReceiveTelemetry");


try
{
    while (true)
    {
        var telemetry = simulator.GenerateTelemetry(deviceId, 
                                                    deviceName, 
                                                    nominalPower,
                                                    currentVersion, 
                                                    deviceType);
        var (success, errorMessage) = await publisher.PublishSafeAsync(telemetry);

        if (success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            ConsoleUI.PrintSuccess(telemetry.CurrentPower, telemetry.NominalPower);
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleUI.PrintError(errorMessage ?? "Unknown error");
            Console.ResetColor();
        }

        await Task.Delay(delayMs);
    }
}
catch (OperationCanceledException)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("\n[SYSTEM] Simulation stopped by user.");
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    ConsoleUI.PrintCritical($"Fatal error: {ex.Message}");
    Console.ResetColor();
    Environment.Exit(1);
}