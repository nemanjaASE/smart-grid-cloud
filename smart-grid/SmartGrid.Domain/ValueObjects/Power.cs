using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.ValueObjects;

public sealed record Power : IComparable<Power>
{
    public double Value { get; }

    private Power(double value)
    {
        Value = value;
    }

    public static Result<Power> Create(double value)
    {
        if (value <= 0)
            Result<Power>.Failure("Power must be positive.", ErrorType.Validation);

        return Result<Power>.Success(new Power(value));
    }

    public int CompareTo(Power? other) =>
        other is null ? 1 : Value.CompareTo(other.Value);

    public static bool operator >(Power left, Power right) => left.Value > right.Value;
    public static bool operator <(Power left, Power right) => left.Value < right.Value;
    public static bool operator >=(Power left, Power right) => left.Value >= right.Value;
    public static bool operator <=(Power left, Power right) => left.Value <= right.Value;

    public static implicit operator double(Power power) => power.Value;

    public override string ToString() => $"{Value} W";
}
