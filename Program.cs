using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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

            var service = new ServiceCollection()
                .AddMediatR(Assembly.GetAssembly(typeof(Program)))
                .BuildServiceProvider();
            var bus = new MediatR.Mediator(type => service.GetService(type));
            var dispatcher = new EventDispatcher(conn, bus);

            var repo= new AggregateRepository(conn);
            var handler = new CreatePlayerCommandHandler(repo, dispatcher);

            var id = Guid.NewGuid();
            var streamId = $@"Player-{id}";
            var cmd = new CreatePlayerCommand(streamId, id);
            await handler.Handle(cmd);


            var player = await repo.Fetch<PlayerAggregate>(streamId);

            await dispatcher.RaiseEvent(player,
                new NicknameAddedEvent {Nickname = "Chr", StreamId = player.StreamId},
                new AddressAddedEvent {HomeAddress = new Address {HomeAddress = "Sandøgade 4, 8200 Aarhus N"}, StreamId = player.StreamId});

            return player.Id;
        }
    }
}
