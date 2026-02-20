using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using SmartGrid.Application.Common;
using SmartGrid.Application.Interfaces.Storage;

namespace SmartGrid.Infrastructure.Persistence.AzureBlob
{
    internal abstract class AzureBlobStorage<TMetadata>(BlobContainerClient containerClient, ILogger logger)
        : IFileStorage<TMetadata>
    {
        protected readonly BlobContainerClient _containerClient = containerClient;
        protected readonly ILogger _logger = logger;

        public abstract string GetBlobPath(TMetadata metadata);

        private static readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

        public async Task<bool> ExistsAsync(string blobPath, CancellationToken ct = default)
        {
            var blobClient = _containerClient.GetBlobClient(blobPath);

            var respone = await blobClient.ExistsAsync(ct);

            return respone.Value;
        }

        public async Task SaveAsync(FileData<TMetadata> file, CancellationToken ct = default)
        {
            var blobPath = GetBlobPath(file.Metadata);

            var blobClient = _containerClient.GetBlobClient(blobPath);

            using var stream = new MemoryStream(file.Content);

            _logger.LogInformation("Uploading to container: {Container}, Path: {Path}",
                _containerClient.Name, blobPath);

            var blobOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = GetContentType(blobPath)
                }
            };

            await blobClient.UploadAsync(stream, blobOptions, ct);
        }
        public async Task<bool> ExistsAsync(TMetadata metadata, CancellationToken ct = default)
        {
            var blobPath = GetBlobPath(metadata);
            var blobClient = _containerClient.GetBlobClient(blobPath);
            var response = await blobClient.ExistsAsync(ct);
            return response.Value;
        }
        public async Task DeleteAsync(TMetadata metadata, CancellationToken ct = default)
        {
            var blobPath = GetBlobPath(metadata);
            var blobClient = _containerClient.GetBlobClient(blobPath);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots, cancellationToken: ct);
        }
        private static string GetContentType(string path)
        {
            if (_contentTypeProvider.TryGetContentType(path, out var contentType))
            {
                return contentType!;
            }

            return "application/octet-stream"; // fallback
        }
    }
}
