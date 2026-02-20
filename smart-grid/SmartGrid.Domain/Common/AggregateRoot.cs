namespace SmartGrid.Domain.Common
{
    public abstract class AggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
