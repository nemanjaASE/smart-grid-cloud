namespace SmartGrid.Domain.Common
{
    public interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
