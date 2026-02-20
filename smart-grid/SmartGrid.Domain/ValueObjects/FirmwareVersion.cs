using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.ValueObjects;

public sealed record FirmwareVersion : IComparable<FirmwareVersion>
{
    public string Value { get; }
    private readonly Version _parsedVersion;

    private FirmwareVersion(string value, Version parsedVersion)
    {
        Value = value;
        _parsedVersion = parsedVersion;
    }

    public static Result<FirmwareVersion> Create(string version)
    {
        if (string.IsNullOrWhiteSpace(version))
            return Result<FirmwareVersion>.Failure("Firmware version cannot be empty.", ErrorType.Validation);

        if (!version.StartsWith("V", StringComparison.OrdinalIgnoreCase))
            return Result<FirmwareVersion>.Failure("Firmware version must start with 'V'.", ErrorType.Validation);

        var cleanVersion = version[1..];
        if (!Version.TryParse(cleanVersion, out var parsedVersion))
            return Result<FirmwareVersion>.Failure("Firmware version format is invalid.", ErrorType.Validation);

        return Result<FirmwareVersion>.Success(new FirmwareVersion(version, parsedVersion));
    }
    public int CompareTo(FirmwareVersion? other)
    {
       return other is null ? 1 : _parsedVersion.CompareTo(other._parsedVersion);
    }
    public static bool operator >(FirmwareVersion left, FirmwareVersion right) => left.CompareTo(right) > 0;
    public static bool operator <(FirmwareVersion left, FirmwareVersion right) => left.CompareTo(right) < 0;
    public override string ToString() => Value;
}
