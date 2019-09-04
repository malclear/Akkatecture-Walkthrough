using Akkatecture.Aggregates;
using Akkatecture.Core;

namespace Domain.Sagas.MoneyTransfer.Events
{
    public class EventWasSeen: AggregateEvent<MoneyTransferSaga, MoneyTransferSagaId>
    {
        public IIdentity DomainEventId { get; } 
        public EventWasSeen(IIdentity domainEventId)
        {
            DomainEventId = domainEventId;
        }
    }
}