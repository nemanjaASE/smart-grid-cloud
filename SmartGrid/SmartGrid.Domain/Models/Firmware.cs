using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Models
{
    public class Firmware
    {
        public EntityId Id { get; private set; }
        public DeviceType DeviceType { get; private set; } = DeviceType.Unknown;
        public FirmwareVersion Version { get; private set; }

        public FirmwareFileName FileName { get; private set; }
        public long FileSizeInBytes { get; private set; }
       
        public DateTime UploadedAt { get; private set; }

        private Firmware(
            EntityId id,
            DeviceType deviceType,
            FirmwareVersion version,
            FirmwareFileName filenaName,
            long fileSizeInBytes,
            DateTime uploadedAt)
        {
            Id = id;
            DeviceType = deviceType;
            Version = version;
            FileName = filenaName;
            FileSizeInBytes = fileSizeInBytes;
            UploadedAt = uploadedAt;
        }

        #region Factory Method

        public static Result<Firmware> Create(
            DeviceType deviceType, 
            string version, 
            string fileName, 
            long fileSize,
            DateTime uploadedAt
        )
        {
            if (!Enum.IsDefined(typeof(DeviceType), deviceType) || deviceType == DeviceType.Unknown)
                return Result<Firmware>.Failure("A valid and defined DeviceType must be specified.",
                    ErrorType.Validation);

            var versionResult = FirmwareVersion.Create(version);
            if (versionResult.IsFailure)
                return Result<Firmware>.Failure(versionResult.Error!.Message, ErrorType.Validation);

            var fileNameResult = FirmwareFileName.Create(fileName);
            if (fileNameResult.IsFailure)
                return Result<Firmware>.Failure(fileNameResult.Error!.Message, ErrorType.Validation);

            if (fileSize <= 0)
                return Result<Firmware>.Failure("Firmware file cannot be empty.",
                    ErrorType.Validation);

            return Result<Firmware>.Success(new Firmware(
                            EntityId.New(),
                            deviceType,
                            versionResult.Value,
                            fileNameResult.Value,
                            fileSize,
                            uploadedAt
            ));
        }
        public static Result<Firmware> Load(
            string id,
            DeviceType deviceType,
            string version,
            string fileName,
            long fileSize,
            DateTime uploadedAt)
        {
            var entityIdResult = EntityId.Create(id);
            if (entityIdResult.IsFailure)
                return Result<Firmware>.Failure(entityIdResult.Error!.Message, ErrorType.Validation);

            var versionResult = FirmwareVersion.Create(version);
            if (versionResult.IsFailure)
                return Result<Firmware>.Failure(versionResult.Error!.Message, ErrorType.Validation);

            var fileNameResult = FirmwareFileName.Create(fileName);
            if (fileNameResult.IsFailure)
                return Result<Firmware>.Failure(fileNameResult.Error!.Message, ErrorType.Validation);

            return Result<Firmware>.Success(new Firmware(
                entityIdResult.Value,
                deviceType,
                versionResult.Value,
                fileNameResult.Value,
                fileSize,
                uploadedAt
            ));
        }

        #endregion
    }
}
