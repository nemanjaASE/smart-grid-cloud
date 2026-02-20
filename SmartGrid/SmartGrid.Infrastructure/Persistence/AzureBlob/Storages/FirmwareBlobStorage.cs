using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartGrid.Application.Common;
using SmartGrid.Application.Common.Extensions;
using SmartGrid.Application.Interfaces.Storage;
using SmartGrid.Infrastructure.Common.Options;

namespace SmartGrid.Infrastructure.Persistence.AzureBlob.Storages
{
    internal class FirmwareBlobStorage(
        BlobServiceClient blobServiceClient,
        ILogger<FirmwareBlobStorage> logger,
        IOptions<AzureBlobOptions> options
    ) 
        : AzureBlobStorage<FirmwareMetadata>(
            blobServiceClient.GetBlobContainerClient(options.Value.FirmwareBlob),
            logger),
          IFirmwareBlobStorage
    {
        public override string GetBlobPath(FirmwareMetadata metadata)
        {
            return FirmwarePathExtensions.GetFirmwareBlobPath(metadata);
        }
    }
}
