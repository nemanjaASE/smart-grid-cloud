using SmartGrid.Domain.Common;
using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.ValueObjects;

public sealed record EntityId
{
    public string Value { get; }

    private EntityId(string value)
    {
        Value = value;
    }

    public static Result<EntityId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<EntityId>.Failure("Id format is invalid.", ErrorType.Validation);

        if (!Guid.TryParse(value, out _))
            return Result<EntityId>.Failure("Invalid DeviceId format.", ErrorType.Validation);

        return Result<EntityId>.Success(new EntityId(value));
    }

    public static EntityId New() => new(Guid.NewGuid().ToString());

    public override string ToString() => Value;

    public static implicit operator string(EntityId id) => id.Value;
}
