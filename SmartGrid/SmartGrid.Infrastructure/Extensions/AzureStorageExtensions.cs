using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using SmartGrid.Application.Interfaces.Messaging;
using SmartGrid.Application.Interfaces.Repositories;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Domain.Models;
using SmartGrid.Infrastructure.Persistence.AzureBlob.Storages;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Services;
using SmartGrid.Infrastructure.Persistence.AzureTable.Common;
using SmartGrid.Infrastructure.Persistence.AzureTable.Entities;
using SmartGrid.Infrastructure.Persistence.AzureTable.KeyProviders;
using SmartGrid.Infrastructure.Persistence.AzureTable.Mappers;
using SmartGrid.Infrastructure.Persistence.AzureTable.Repositories;

namespace SmartGrid.Infrastructure.Extensions
{
    internal static class AzureStorageExtensions
    {
        public static IServiceCollection AddAzureTables(
            this IServiceCollection services,
            string connectionString)
        {
            // Table Service Client
            services.AddSingleton(new TableServiceClient(connectionString));

            // Mappers
            services.AddSingleton<ITableMapper<Telemetry, TelemetryEntity>, TelemetryTableMapper>();
            services.AddSingleton<ITableMapper<Device, DeviceEntity>, DeviceTableMapper>();
            services.AddSingleton<ITableMapper<DeviceStatus, DeviceStatusEntity>, DeviceStatusTableMapper>();
            services.AddSingleton<ITableMapper<Firmware, FirmwareEntity>, FirmwareTableMapper>();

            // Key Providers
            services.AddSingleton<ITableKeyProvider<Telemetry>, TelemetryTableKeyProvider>();
            services.AddSingleton<ITableKeyProvider<Device>, DeviceTableKeyProvider>();
            services.AddSingleton<ITableKeyProvider<DeviceStatus>, DeviceStatusTableKeyProvider>();
            services.AddSingleton<ITableKeyProvider<Firmware>, FirmwareTableKeyProvider>();

            // Repositories
            services.AddScoped<ITelemetryRepository, TelemetryRepository>();
            services.AddScoped<IDeviceRepository, DeviceRepository>();
            services.AddScoped<IFirmwareRepository, FirmwareRepository>();
            services.AddScoped<IDeviceStatusQueryRepository, DeviceStatusQueryRepository>();

            return services;
        }
        public static IServiceCollection AddAzureBlobs(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton(sp => new BlobServiceClient(connectionString));

            services.AddScoped<IFirmwareBlobStorage, FirmwareBlobStorage>();

            return services;
        }
        public static IServiceCollection AddAzureQueues(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton(sp =>
            {
                return new QueueServiceClient(connectionString, new QueueClientOptions
                {
                    MessageEncoding = QueueMessageEncoding.Base64
                });
            });

            services.AddScoped<IAlertQueueService, AlertQueueService>();
            services.AddScoped<IDeviceStatusQueueService, DeviceStatusQueueService>();

            return services;
        }
    }
}
