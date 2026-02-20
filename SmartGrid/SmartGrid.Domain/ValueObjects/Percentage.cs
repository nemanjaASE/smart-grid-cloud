using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.ValueObjects;

public sealed record Percentage : IComparable<Percentage>
{
    public double Value { get; }

    private Percentage(double value) => Value = value;
    public static Percentage Zero() => new(0);
    public static Percentage FromRaw(double value) =>
        new(Math.Clamp(value, 0, 150));
    public static Result<Percentage> Create(double value)
    {
        if (value < 0 || value > 150)
            return Result<Percentage>.Failure("Percentage must be 0-150.", ErrorType.Validation);
        return Result<Percentage>.Success(new Percentage(value));
    }
    public int CompareTo(Percentage? other) =>
        other is null ? 1 : Value.CompareTo(other.Value);
    public static bool operator >(Percentage left, Percentage right) => left.Value > right.Value;
    public static bool operator <(Percentage left, Percentage right) => left.Value < right.Value;
    
    public static implicit operator double(Percentage p) => p.Value;
    public override string ToString() => $"{Value}%";
}
