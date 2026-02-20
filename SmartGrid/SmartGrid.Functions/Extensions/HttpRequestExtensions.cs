using Microsoft.AspNetCore.Http;
using SmartGrid.Application.Features.Firmwares;
using SmartGrid.Application.Features.Firmwares.Commands;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Functions.Extensions
{
    internal static class HttpRequestExtensions
    {
        public static async Task<Result<UploadFirmwareCommand>> ToUploadFirmwareCommandAsync(this HttpRequest req)
        {
            if (!req.HasFormContentType)
            {
                return Result<UploadFirmwareCommand>.Failure("Expected multipart/form-data.", ErrorType.Validation);
            }

            try
            {
                var form = await req.ReadFormAsync();

                string deviceTypeRaw = form["DeviceType"].ToString().Trim('"');
                Enum.TryParse<DeviceType>(deviceTypeRaw, ignoreCase: true, out var deviceType);

                var firmwareFile = form.Files.GetFile("FirmwareFile");

                if (firmwareFile == null || firmwareFile.Length == 0)
                    return Result<UploadFirmwareCommand>.Failure("Firmware file is required.", ErrorType.Validation);

                var fileBytes = await firmwareFile.ToByteArrayAsync();

                var uploadedFile = new UploadedFirmwareFile
                {
                    FileName = firmwareFile.FileName,
                    Content = fileBytes
                };

                return Result<UploadFirmwareCommand>.Success(new UploadFirmwareCommand
                {
                    FirmwareFile = uploadedFile,
                    DeviceType = deviceType,
                    Version = form["Version"].ToString().Trim('"')
                });
            }
            catch (Exception ex)
            {
                return Result<UploadFirmwareCommand>.Failure($"HTTP Parsing Error: {ex.Message}",
                    ErrorType.Failure);
            }
        }
    }
}
