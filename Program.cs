using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Newtonsoft.Json;

namespace PlayerDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var id = await CreatePlayer();

            using var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            await conn.ConnectAsync();

            var streamId = $"Player-{id}";
            var eventSlice = await conn.ReadStreamEventsForwardAsync(streamId, 0, 100, resolveLinkTos: false);
            var player = new Player();
            foreach (var ev in eventSlice.Events)
            {
                var eventType= Type.GetType(ev.Event.EventType);
                var eventJson = Encoding.UTF8.GetString(ev.Event.Data);
                var @event = JsonConvert.DeserializeObject(eventJson, eventType) as IEvent<Player>;
                @event.Apply(player);
            }
            Console.WriteLine(player.ToString());
        }

        private static async Task<Guid> CreatePlayer()
        {
            using var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            await conn.ConnectAsync();

            var player = new Player();
            var createdEvent = new PlayerCreatedEvent
            {
                Id = Guid.NewGuid()
            };
            createdEvent.Apply(player);

            var nicknameAdded = new NicknameAddedEvent {Nickname = "Chr"};
            nicknameAdded.Apply(player);

            var addressAdded = new AddressAddedEvent {HomeAddress = new Address {HomeAddress = "Sandøgade 4, 8200 Aarhus N"}};
            addressAdded.Apply(player);

            await conn.AppendToStreamAsync(
                $"Player-{player.Id}",
                ExpectedVersion.Any,
                new EventData(
                    Guid.NewGuid(),
                    typeof(PlayerCreatedEvent).AssemblyQualifiedName,
                    isJson: true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(createdEvent)),
                    null
                ),
                new EventData(
                    Guid.NewGuid(),
                    typeof(NicknameAddedEvent).AssemblyQualifiedName,
                    isJson: true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(nicknameAdded)),
                    null
                )
                ,
                new EventData(
                    Guid.NewGuid(),
                    typeof(AddressAddedEvent).AssemblyQualifiedName,
                    isJson: true,
                    Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(addressAdded)),
                    null
                )
            );

            return player.Id;
        }
    }

    public interface IEvent<TAggregate>
    {
        void Apply(TAggregate agg);
    }

    public class PlayerCreatedEvent : IEvent<Player>
    {
        public Guid Id { get; set; }
        public void Apply(Player agg)
        {
            agg.Id = Id;
        }
    }

    public class NicknameAddedEvent : IEvent<Player>
    {
        public string Nickname { get; set; }
        public void Apply(Player agg)
        {
            agg.NickName = Nickname;
        }
    }

    public class AddressAddedEvent : IEvent<Player>
    {
        public Address HomeAddress { get; set; }
        public void Apply(Player agg)
        {
            agg.Address = HomeAddress;
        }
    }

    public class Player
    {
        public string NickName { get; set; }
        public Guid Id { get; set; }

        public Address Address { get; set; }
        
        public override string ToString()
        {
            return $"Id: {Id}, Nickname: {NickName}";
        }
    }

    public class Address
    {
        public string HomeAddress { get; set; }
    }
}
