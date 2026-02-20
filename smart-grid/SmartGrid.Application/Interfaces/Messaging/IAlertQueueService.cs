using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Application.Interfaces.Messaging
{
    public interface IAlertQueueService
    {
        Task SendAlertAsync(Alert alert, CancellationToken ct = default);
    }
}
