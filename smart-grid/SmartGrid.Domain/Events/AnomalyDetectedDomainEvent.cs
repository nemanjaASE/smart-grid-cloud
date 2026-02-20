using SmartGrid.Domain.Common;
using SmartGrid.Domain.ValueObjects;

namespace SmartGrid.Domain.Events
{
    public sealed class AnomalyDetectedDomainEvent(Alert alert, DateTime occurredOn) : IDomainEvent
    {
        public Alert Alert { get; set; } = alert;
        public DateTime OccurredOn { get; } = occurredOn;
    }
}
