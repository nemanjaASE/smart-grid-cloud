using SmartGrid.Domain.Common;

namespace SmartGrid.Domain.ValueObjects;
public sealed record Message
{
    public string Value { get; }

    private Message(string value)
    {
        Value = value;
    }

    public static Result<Message> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Result<Message>.Failure("Message cannot be empty.");

        if (value.Length > 256)
            return Result<Message>.Failure("Message cannot exceed 256 characters.");

        return Result<Message>.Success(new Message(value));
    }

    public override string ToString() => Value;

    public static implicit operator string(Message message) => message.Value;
}