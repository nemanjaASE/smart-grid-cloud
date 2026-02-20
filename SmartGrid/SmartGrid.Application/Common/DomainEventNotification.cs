using MediatR;
using SmartGrid.Domain.Common;

namespace SmartGrid.Application.Common
{
    public sealed class DomainEventNotification<TDomainEvent>(TDomainEvent domainEvent) : INotification
            where TDomainEvent : IDomainEvent
    {
        public TDomainEvent Event { get; } = domainEvent;
    }
}
