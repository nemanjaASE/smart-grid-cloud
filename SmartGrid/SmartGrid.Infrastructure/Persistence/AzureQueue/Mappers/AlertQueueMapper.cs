using SmartGrid.Domain.ValueObjects;
using SmartGrid.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartGrid.Infrastructure.Persistence.AzureQueue.Mappers
{
    public static class AlertQueueMapper
    {
        public static AlertMessage? ToQueueMessage(this Alert model)
        {
            if (model == null) return null;

            return new AlertMessage
            {
                DeviceId = model.DeviceId.Value,
                AlertType = model.AlertType,
                Message = model.Message.Value,
                Timestamp = model.Timestamp
            };
        }

        public static Alert? ToDomainModel(this AlertMessage message)
        {
            var alertResult = Alert.Load(
                message.DeviceId,
                message.AlertType,
                message.Message,
                message.Timestamp
            );

            if (alertResult.IsFailure)
                return null;

            return alertResult.Value;
        }
    }
}
