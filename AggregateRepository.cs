using System;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace PlayerDemo
{
    public class AggregateRepository
    {
        private readonly IEventStoreConnection _conn;

        public AggregateRepository(IEventStoreConnection conn)
        {
            _conn = conn;
        }
        public async Task<TAggregate> Fetch<TAggregate>(string streamId) where TAggregate : AggregateBase, new()
        {
            var eventSlice = await _conn.ReadStreamEventsForwardAsync(streamId, 0, 100, resolveLinkTos: false);
            var player = new TAggregate() { StreamId = streamId };
            foreach (var ev in eventSlice.Events)
            {
                var eventType = Type.GetType(ev.Event.EventType);
                var eventJson = Encoding.UTF8.GetString(ev.Event.Data);
                var @event = JsonConvert.DeserializeObject(eventJson, eventType) as IEvent<TAggregate>;
                if (@event is object)
                    @event.Apply(player);
                else
                    Console.WriteLine("🤯 😱");
            }

            return player;
        }
    }
}