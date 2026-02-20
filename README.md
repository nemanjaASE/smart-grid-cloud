# Smart Grid Cloud

A cloud-native system for monitoring and early risk detection in power grids, based on telemetry data from smart inverter devices. Developed as laboratory exercise material for the **"Cloud Application Development in Smart Grids"** course at the Faculty of Technical Sciences, Novi Sad.

![.NET 8.0](https://img.shields.io/badge/.NET_8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET Core Web API](https://img.shields.io/badge/ASP.NET_Core_Web_API-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Azure Functions](https://img.shields.io/badge/Azure_Functions-0062AD?style=for-the-badge&logo=azurefunctions&logoColor=white)
![Azure Table Storage](https://img.shields.io/badge/Table_Storage-0089D6?style=for-the-badge&logo=microsoftazure&logoColor=white)
![Azure Blob Storage](https://img.shields.io/badge/Blob_Storage-0089D6?style=for-the-badge&logo=microsoftazure&logoColor=white)
![Azure Queue Storage](https://img.shields.io/badge/Queue_Storage-0089D6?style=for-the-badge&logo=microsoftazure&logoColor=white)
![SignalR](https://img.shields.io/badge/SignalR-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![MediatR](https://img.shields.io/badge/MediatR-512BD4?style=for-the-badge&logo=nuget&logoColor=white)
![Clean Architecture](https://img.shields.io/badge/Clean_Architecture-000000?style=for-the-badge)
![DDD](https://img.shields.io/badge/DDD-0078D4?style=for-the-badge)
![CQRS](https://img.shields.io/badge/CQRS-ED8B00?style=for-the-badge)
![FluentValidation](https://img.shields.io/badge/FluentValidation-CC0000?style=for-the-badge&logo=nuget&logoColor=white)
![Node.js](https://img.shields.io/badge/Node.js-339933?style=for-the-badge&logo=nodedotjs&logoColor=white)
![Postman](https://img.shields.io/badge/Postman-FF6C37?style=for-the-badge&logo=postman&logoColor=white)

---

## 📋 Table of Contents
- [System Architecture](#-system-architecture)
- [Technologies](#-technologies)
  - [Core Framework](#core-framework)
  - [Cloud Infrastructure](#cloud-infrastructure-microsoft-azure)
  - [Patterns & Validation](#patterns--validation)
- [System Components](#-system-components)
  - [Inverter Telemetry Simulator](#-inverter-telemetry-simulator)
  - [Azure Functions – Serverless Backend](#-azure-functions--serverless-backend)
  - [Web API & Background Worker](#-web-api--background-worker)
  - [Dashboard](#-dashboard)
- [Clean Code Principles](#-clean-code-principles)
- [Prerequisites](#-prerequisites)
- [Local Environment Setup](#-local-environment-setup)
  - [Infrastructure Initialization](#infrastructure-initialization-tools-project)
  - [Running the Backend](#running-the-backend)
  - [Running the Inverter Simulator](#running-the-inverter-simulator)
  - [Running the Dashboard](#running-the-dashboard)
  - [Deployment to Azure](#deployment-to-azure)
- [License](#-license)
- [Contributing](#-contributing)

---

## 🏗 System Architecture

The system is built following the principles of **Clean Architecture** and **Domain-Driven Design (DDD)**, organized into four layers:

| Layer | Role |
|---|---|
| **Domain** (Core) | Defines business logic, rich domain models (Entities), and Value Objects. Independent of any external technologies or frameworks. |
| **Application** (Use Cases) | Orchestrates business logic using the CQRS pattern via the MediatR library. |
| **Infrastructure** (Technical Details) | Implements interfaces for data persistence (Azure Table Storage), file storage (Blob Storage), and messaging (Queue Storage). |
| **Presentation** (Entry Point) | Azure Functions and an ASP.NET Core Web API that expose functionalities to clients. |

<img width="1708" height="962" alt="Clean Architecture Folder Structure" src="https://github.com/user-attachments/assets/11d0ef60-4078-4e01-a45f-c3f3e26dfa55" />

---

## 🚀 Technologies

### Core Framework

- **.NET 8.0** – Developed using the Isolated Worker model for Azure Functions, providing decoupled execution and full control over dependencies.

### Cloud Infrastructure (Microsoft Azure)

| Service | Purpose |
|---|---|
| ⚡ **Azure Functions** | Serverless logic handling HTTP, Timer, Blob, and Queue triggers. |
| 🗄️ **Table Storage** | Low-cost NoSQL storage for measurement history, device metadata, and system snapshots. |
| 📁 **Blob Storage** | Secure storage for binary firmware files used in Over-The-Air (OTA) updates. |
| ✉️ **Queue Storage** | Asynchronous messaging for reliable processing of alerts and status updates. |
| 🔄 **SignalR** | Real-time, bi-directional communication between the backend and the Dashboard. |

### Patterns & Validation

- 🎯 **MediatR** – Implementation of the Mediator pattern to decouple components, including Pipeline Behaviors for cross-cutting concerns.
- ✅ **FluentValidation** – Robust input validation to ensure data integrity across the system.

---

## 💻 System Components

The system is architected into four main layers, ensuring scalability and real-time responsiveness.

![System Architecture Diagram](https://github.com/user-attachments/assets/441dfe94-d771-44aa-aa88-51e9828fd57c)

### 🔌 Inverter Telemetry Simulator

> **Role:** Simulates field devices (Smart Inverters).

- Handles device **registration** and periodically pushes telemetry data (power output, voltage, timestamp) to the cloud via HTTP.
- Provides a consistent data stream for testing system load and anomaly detection.

---

### ⚡ Azure Functions – Serverless Backend

Acts as the event-driven core of the system, processing data through various triggers:

| Function | Trigger | Description |
|---|---|---|
| **ReceiveTelemetry** | HTTP | Ingests raw data, calculates load percentages, and triggers downstream processing. |
| **GridMonitor** | Timer (10s) | Performs periodic health checks to detect anomalies like grid overloads or offline devices. |
| **FirmwareProcessor** | Blob | Automates the OTA update flow whenever a new firmware binary is uploaded. |
| **ProcessAlerts** | Queue | Decouples critical alarm logging and processing from the main data ingestion flow. |

---

### 🌐 Web API & Background Worker

- **SignalR Hub** (`DeviceHub`) – Manages real-time, bi-directional communication with the dashboard.
- **DeviceStatusWorker** – A dedicated background service that polls the status queue and broadcasts live updates to the UI.
- **REST API** – Provides management endpoints for device metadata, historical telemetry, and firmware management.

---

### 📊 Dashboard

> **Tech:** Node.js-based web application.

| Feature | Description |
|---|---|
| **Live Grid State** | Real-time visualization of the power grid status. |
| **Device Management** | List and monitor all registered smart inverters. |
| **Firmware Interface** | Dedicated UI for uploading and deploying firmware updates across the grid. |

---

## 🛠 Clean Code Principles

The project strictly adheres to clean code standards:

- **Result Pattern** – Used for business logic flow control instead of throwing exceptions.
- **Meaningful Naming** – Classes, methods, and variables use descriptive, self-documenting names.
- **Single Responsibility** – Every class and method has one primary purpose.
- **Eventual Consistency** – Balances performance and data consistency in a distributed cloud environment.

---

## 📋 Prerequisites

- **Visual Studio 2022** (v17.x) with the Azure development workload.
- **Azure for Students** account, or local emulators (Azurite for Storage + Azure Storage Explorer).
- **Node.js** for running the Dashboard.

---

## 🖥 Local Environment Setup

Before running the applications, configure the storage connection strings:

- **Start Azurite** to emulate Azure Storage services locally.
- **Azure Functions project** – populate `local.settings.json` with your storage credentials (use `UseDevelopmentStorage=true` for local work).
- **Web API project** – fill `appsettings.json` with the corresponding `AzureOptions` configuration.

A. **Azure Functions** (_local.settings.json_)
Create or update the _local.settings.json_ file in the root of your Azure Functions project. This file configures the serverless runtime, trigger schedules, and connection details for all storage services (Table, Blob, and Queue).

```
 {
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "GridMonitorSchedule": "*/10 * * * * *",
    "AzureTableOptions:ConnectionString": "UseDevelopmentStorage=true",
    "AzureTableOptions:TelemetriesTable": "Telemetries",
    "AzureTableOptions:DevicesTable": "Devices",
    "AzureTableOptions:DeviceStatusesTable": "DeviceStatuses",
    "AzureTableOptions:FirmwaresTable": "Firmwares",
    "AzureBlobOptions:FirmwareBlob": "firmware-updates",
    "AzureBlobOptions:ConnectionString": "UseDevelopmentStorage=true",
    "AzureQueueOptions:AlertQueue": "alert-queue",
    "AzureQueueOptions:DeviceStatusQueue": "device-status-queue",
    "AzureQueueOptions:ConnectionString": "UseDevelopmentStorage=true",
    "ParallelSettings:MaxDegreeOfParallelism": 20
  }
}
```

B. **ASP.NET Core Web API** (_appsettings.json_)
Update the appsettings.json file in your Web API project. This configuration allows the Web API to host the SignalR Hub, manage CORS for the Dashboard, and enables the Background Worker to poll the status queue.

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "WebApi": {
    "CorsPolicyName": "_reactAppPolicy",
    "ReactOrigin": "http://localhost:5173",
    "DeviceHubRoute": "/device-status-hub"
  },
  "AzureQueueOptions": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "DeviceStatusQueue": "device-status-queue"
  },
  "AzureTableOptions": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "DevicesTable": "Devices",
    "FirmwaresTable": "Firmwares",
    "DeviceStatusesTable": "DeviceStatuses"
  },
  "AzureBlobOptions": {
    "ConnectionString": "UseDevelopmentStorage=true",
    "FirmwareBlob": "firmware-updates"
  },
  "ParallelSettings": {
    "MaxDegreeOfParallelism": 20
  }
}
```
### Infrastructure Initialization (Tools Project)

Before starting the Azure Functions or the Web API, you must initialize the storage resources in your local emulator (Azurite) or Azure account. This is done via the **Tools** console application.

<img width="2324" height="808" alt="Screenshot 2026-02-20 102156" src="https://github.com/user-attachments/assets/445b943d-3124-4df5-8381-54307ea36ade" />

---

### Running the Backend

1. Open the solution in **Visual Studio 2022**.
2. Set both the **Azure Functions** and **Web API** projects as startup projects.
3. Press **F5**.
   - The Azure Functions Core Tools console will open, listing all detected function endpoints (e.g., `ReceiveTelemetry`, `GridMonitor`).
   - The Web API will start, hosting the SignalR Hub and REST endpoints.

<img width="2316" height="1176" alt="Screenshot 2026-02-20 102519" src="https://github.com/user-attachments/assets/857ea1eb-df68-49bf-8d7f-11a83672ec1b" />


<img width="2332" height="670" alt="Screenshot 2026-02-20 102720" src="https://github.com/user-attachments/assets/8be6ddc8-e927-422a-9343-d384cb51d4b4" />

---

### Running the Inverter Simulator

1. Run the **Inverter Telemetry Simulator** (Console Application).
2. Follow the prompts to register a device — enter `DeviceName`, `NominalPower`, `DeviceType` (Solar Panel or Wind Turbine), and `Location`.
3. Once registered, the simulator enters a loop, sending telemetry data every **10 seconds** to the cloud.

<img width="2332" height="1002" alt="Screenshot 2026-02-20 103916" src="https://github.com/user-attachments/assets/c2281219-8abe-4b8c-93bf-25313fdf4990" />

---

### Running the Dashboard

1. Open a terminal in the Dashboard project directory.
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the application:
   ```bash
   npm run dev
   ```
4. The dashboard will be available at the address shown in the terminal (typically `http://localhost:5173`).

<img width="1430" height="748" alt="Picture2" src="https://github.com/user-attachments/assets/9729244d-482d-4269-8505-a3f61de40e7c" />

<img width="1430" height="747" alt="Picture1" src="https://github.com/user-attachments/assets/99f1f553-5b5e-446a-ace0-b2b9bbf6c15b" />

<img width="1430" height="747" alt="Picture3" src="https://github.com/user-attachments/assets/3443d096-a696-4144-a0f5-56af4663963e" />

---

### Deployment to Azure

1. **Azure Portal** – Create a Function App (Consumption plan) and a Storage Account.
2. **Visual Studio** – Right-click the project → **Publish** → Select Azure Function App → Choose your created resource.
3. **Configuration** – Update the cloud endpoint URLs in the Simulator and Dashboard to point to your deployed Azure services.

---

## 📄 License

This project is the intellectual property of the author. It is distributed under the **MIT License**. See the `LICENSE` file for more information.

> **Note:** This system was designed and developed by the author as part of the "Cloud Application Development in Smart Grids" course at the Faculty of Technical Sciences, Novi Sad. While the project was created within an academic framework, all implementation details, architecture design, and source code are the sole property of the author.

---

## 🤝 Contributing

Contributions are what make the academic community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

---
