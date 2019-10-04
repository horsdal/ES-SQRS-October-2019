using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using MediatR;
using Newtonsoft.Json;

namespace PlayerDemo
{
    public class EventDispatcher
    {
        private readonly IEventStoreConnection _conn;
        private readonly IMediator _bus;

        public EventDispatcher(IEventStoreConnection conn, IMediator bus)
        {
            _conn = conn;
            _bus = bus;
        }

        public async Task RaiseEvent<TAggregate>(TAggregate agg, params IEvent<TAggregate>[] events)  where TAggregate : AggregateBase
        {
            await _conn.AppendToStreamAsync(
                agg.StreamId,
                agg.Version,
                events.Select(e =>
                new EventData(
                    Guid.NewGuid(),
                    e.GetType().FullName,
                    isJson: true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e)),
                    null
                )));

            foreach (var @event in events)
            {
                @event.Apply(agg);
                await _bus.Publish(@event);
            }
        }
    }
}