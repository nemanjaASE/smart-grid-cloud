using SmartGrid.Domain.Enums;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue.Messages
{
    public class AlertMessage
    {
        public string DeviceId { get; set; } = string.Empty;
        public AlertType AlertType { get; set; } 
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
