using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.ValueObjects
{
    public sealed record FirmwareFileName
    {
        public string Value { get; }

        private FirmwareFileName(string value)
        {
            Value = value;
        }

        public static Result<FirmwareFileName> Create(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Result<FirmwareFileName>.Failure("File filename cannot be empty.", ErrorType.Validation);

            if (!fileName.EndsWith(".bin", StringComparison.OrdinalIgnoreCase))
                return Result<FirmwareFileName>.Failure("Invalid firmware file extension. Only .bin is supported.", ErrorType.Validation);

            return Result<FirmwareFileName>.Success(new FirmwareFileName(fileName));
        }

        public override string ToString() => Value;

        public static implicit operator string(FirmwareFileName fileName) => fileName.Value;
    }
}
