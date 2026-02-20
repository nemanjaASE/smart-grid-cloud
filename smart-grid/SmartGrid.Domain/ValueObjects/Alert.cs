using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.ValueObjects
{
    public sealed record Alert
    {
        public EntityId DeviceId { get; init; }
        public AlertType AlertType { get; init; }
        public Message Message { get; init; }
        public DateTime Timestamp { get; init; }

        private Alert(EntityId deviceId, AlertType type, Message message, DateTime timestamp)
        {
            DeviceId = deviceId;
            AlertType = type;
            Message = message;
            Timestamp = timestamp;
        }

        public static Result<Alert> Create(string deviceId, AlertType alertType, string message)
        {
            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure) 
                return Result<Alert>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var alertMessageResult = Message.Create(message);
            if (alertMessageResult.IsFailure) 
                return Result<Alert>.Failure(alertMessageResult.Error!.Message, ErrorType.Validation);

            if (!Enum.IsDefined(typeof(AlertType), alertType))
                return Result<Alert>.Failure("Invalid alert type specified.", ErrorType.Validation);

            return Result<Alert>.Success(new Alert(deviceIdResult.Value, alertType, alertMessageResult.Value, DateTime.UtcNow));
        }

        public static Result<Alert> Load(string deviceId, AlertType alertType, string message, DateTime timestamp)
        {
            var deviceIdResult = EntityId.Create(deviceId);
            if (deviceIdResult.IsFailure) 
                return Result<Alert>.Failure(deviceIdResult.Error!.Message, ErrorType.Validation);

            var alertMessageResult = Message.Create(message);
            if (alertMessageResult.IsFailure) 
                return Result<Alert>.Failure(alertMessageResult.Error!.Message, ErrorType.Validation);

            return Result<Alert>.Success(new Alert(deviceIdResult.Value, alertType, alertMessageResult.Value, timestamp));
        }
    }
}