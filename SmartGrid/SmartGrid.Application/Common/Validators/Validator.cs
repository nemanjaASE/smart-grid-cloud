using FluentValidation;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Application.Common.Validators
{
    internal static class Validator
    {
        public static IRuleBuilderOptions<T, DeviceType> IsValidDeviceType<T>(this IRuleBuilder<T, DeviceType> ruleBuilder)
        {
            return ruleBuilder
                            .IsInEnum().WithMessage("Invalid device type.")
                            .NotEqual(DeviceType.Unknown).WithMessage("DeviceType cannot be Unknown.");
        }

        public static IRuleBuilderOptions<T, string> IsValidFirmwareVersion<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("Firmware version is required.")
                .Matches(@"^[Vv]\d+(\.\d+)*$").WithMessage("Version must be in format V1.0, V2.1.3 etc.");
        }
    }
}
