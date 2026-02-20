using SmartGrid.Application.Common;

namespace SmartGrid.Application.Interfaces.Storage
{
    public interface IFileStorage<TMetadata>
    {
        Task SaveAsync(FileData<TMetadata> file, CancellationToken ct = default);
        Task<bool> ExistsAsync(TMetadata metadata, CancellationToken ct = default);
        Task DeleteAsync(TMetadata metadata, CancellationToken ct = default);
    } 
}