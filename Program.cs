using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using PlayerDemo.Player;

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
            var player = await new AggregateRepository(conn).Fetch<PlayerAggregate>(streamId);
            Console.WriteLine(player.ToString());
        }

        private static async Task<Guid> CreatePlayer()
        {
            using var conn = EventStoreConnection.Create(new Uri("tcp://admin:changeit@localhost:1113"));
            await conn.ConnectAsync();

            var dispatcher = new EventDispatcher(conn);

            var id = Guid.NewGuid();
            var player = new PlayerAggregate(){ StreamId = $@"Player-{id}" };

            await dispatcher.RaiseEvent(player,
                new PlayerCreatedEvent { Id = id },
                new NicknameAddedEvent {Nickname = "Chr"},
                new AddressAddedEvent {HomeAddress = new Address {HomeAddress = "Sandøgade 4, 8200 Aarhus N"}});

            return player.Id;
        }
    }
}
