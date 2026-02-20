using Microsoft.AspNetCore.Mvc;
using SmartGrid.Application.Features.Firmwares;
using SmartGrid.Application.Features.Firmwares.Commands;
using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;
using SmartGrid.WebApi.Extensions;

namespace SmartGrid.WebApi.DTOs
{
    public class UploadFirmwareRequest
    {
        [FromForm(Name = "DeviceType")]
        public DeviceType DeviceType { get; set; }

        [FromForm(Name = "Version")]
        public string Version { get; set; } = string.Empty;

        [FromForm(Name = "FirmwareFile")]
        public IFormFile? File { get; set; }

        public async Task<Result<UploadFirmwareCommand>> ToCommandAsync(CancellationToken ct)
        {
            var trimmedVersion = Version.Trim('"').Trim();

            if (string.IsNullOrWhiteSpace(trimmedVersion))
                return Result<UploadFirmwareCommand>.Failure("Version is required.", ErrorType.Validation);

            if (!Enum.IsDefined(typeof(DeviceType), DeviceType))
                return Result<UploadFirmwareCommand>.Failure("Invalid device type.", ErrorType.Validation);

            if (File == null || File.Length == 0)
                return Result<UploadFirmwareCommand>.Failure("File is required.", ErrorType.Validation);

            return Result<UploadFirmwareCommand>.Success(new UploadFirmwareCommand
            {
                DeviceType = DeviceType,
                Version = Version.Trim('"'),
                FirmwareFile = new UploadedFirmwareFile
                {
                    FileName = File.FileName,
                    Content = await File.ToByteArrayAsync(ct)
                }
            });
        }
    }
}